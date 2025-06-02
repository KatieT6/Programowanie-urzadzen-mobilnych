using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Text.Json;
using Communication;
using DataCommon;
using System.Xml.Serialization;
using XMLXSDValidator;


namespace Server;

internal class Server : IServer, IObservable<IBook>
{
    private HttpListener listener = new();
    private string prefix = "http://localhost:5000/ws/";

    public event EventHandler<Request> messageRecieved;
    public object messageRecievedLock = new object();
    public Dictionary<Guid, WebSocket> clientsSockets = new Dictionary<Guid, WebSocket>();
    public object clientsSocketsLock = new object();
    public Dictionary<WebSocket, Guid> socketsClients = new Dictionary<WebSocket, Guid>();
    public object socketsClientsLock = new object();

    XSDValidator xmlXSDValidator = new XSDValidator();

    public Server() 
    {
        listener.Prefixes.Add(prefix);
        listener.Start();
    }

    private void AddClientSocketPair(Guid id, WebSocket socket)
    {
        if (socket == null) return;
        
        lock (clientsSocketsLock)
        {
            if (!clientsSockets.ContainsKey(id))
            {
                clientsSockets.Add(id, socket);
            }
        }
        lock (socketsClientsLock)
        {
            if(!socketsClients.ContainsKey(socket))
            {
                socketsClients.Add(socket, id);
            }
        }
    }
    private void RemoveClientSocketPair(Guid id, WebSocket socket)
    {

        lock (clientsSocketsLock)
        {
            if (clientsSockets.ContainsKey(id))
            {
                clientsSockets.Remove(id);
            }
        }
        lock (socketsClientsLock)
        {
            if (socketsClients.ContainsKey(socket))
            {
                socketsClients.Remove(socket);
            }
        }
    }

    private void InvokeMessgeRecieved(object? sender, Request msg)
    {
        lock (messageRecievedLock)
        {
            messageRecieved?.Invoke(sender, msg);
        }
    }
    private void SendNewClientRequest(Guid clientId)
    {
        NewClientRequest newClientSubrequest = new NewClientRequest(clientId);

        string xmlMessage = "";
        var xmlSerializer = new XmlSerializer(typeof(NewClientRequest));
        using (var stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, newClientSubrequest);
            xmlMessage = stringWriter.ToString();
        }

        Request newClientRequest = new Request("NewClient", xmlMessage);
        SendMessage(clientId, newClientRequest);
        InvokeMessgeRecieved(this, newClientRequest);
    }
    private void SendDelClientRequest(Guid clientId)
    {
        NewClientRequest delClientSubrequest = new NewClientRequest(clientId);

        string xmlMessage = "";
        var xmlSerializer = new XmlSerializer(typeof(NewClientRequest));
        using (var stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, delClientSubrequest);
            xmlMessage = stringWriter.ToString();
        }

        Request delClientRequest = new Request("DelClient", xmlMessage);
        InvokeMessgeRecieved(this, delClientRequest);
    }

    private async Task HandleClient(HttpListenerContext context)
    {
        var wsContext = await context.AcceptWebSocketAsync(null);
        var socket = wsContext.WebSocket;
        var clientId = Guid.NewGuid();
        var buffer = new byte[1024 * 100];

        AddClientSocketPair(clientId, socket);
        SendNewClientRequest(clientId);

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
                if (result.MessageType == WebSocketMessageType.Close) break;
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);

                xmlXSDValidator.Validate(msg, typeof(Request));

                //Console.WriteLine($"Received message from client {clientId}: {msg}");
                Request request;
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(Request));
                    using (var stringReader = new StringReader(msg))
                    {
                        var deserialized = xmlSerializer.Deserialize(stringReader);
                        if(deserialized is not Request deserializedRequest) continue;
                        request = (Request)deserialized;
                    }
                }
                catch
                {
                    //Console.Writeline($"Error deserializing message: {msg}");
                    continue;
                }
                if (request == null) continue;

                InvokeMessgeRecieved(this, request);
            }
        }
        finally
        {
            Console.WriteLine($"Client disconnected: {clientId}");
            RemoveClientSocketPair(clientId, socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            socket.Dispose();

            SendDelClientRequest(clientId);

            ////Console.Writeline($"Client disconnected: {clientId}");
        }
    }

    public void ServerLoop()
    {
        var serverLoopTask = Task.Run(() => ServerLoopImplAsync());
        serverLoopTask.Wait();
    }
    private async Task ServerLoopImplAsync()
    {
        //Console.Writeline("Server loop started.");
        while (true)
        {
            Console.WriteLine("Waiting for a new client connection...");
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                _ = HandleClient(context);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }

    public Task BroadcastMessage(Request request)
    {
        lock (clientsSocketsLock)
        {
            //Console.Writeline($"Broadcasting to {clientsSockets.Count()}");
            foreach (var clientId in clientsSockets.Keys)
            {
                //Console.Writeline($"Broadcasting to {clientId.ToString()}");
                SendMessage(clientId, request);
            }
        }
        return Task.CompletedTask;
    }
    public Task SendMessage(Guid clientId, Request request)
    {
        string xmlMessage = "";
        var xmlSerializer = new XmlSerializer(typeof(Request));
        using (var stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, request);
            xmlMessage = stringWriter.ToString();
        }
        
        //Console.WriteLine($"Sending message to client {clientId}: {xmlMessage}");

        var buffer = Encoding.UTF8.GetBytes(xmlMessage);
        var segment = new ArraySegment<byte>(buffer);

        lock(clientsSocketsLock)
        {
            if (clientsSockets.TryGetValue(clientId, out var socket))
            {
                return socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        return Task.CompletedTask;
    }

    public IDisposable Subscribe(IObserver<IBook> observer)
    {
        throw new NotImplementedException();
    }

    public void StopServer()
    {
        listener.Stop();
        listener.Close();
    }
}
