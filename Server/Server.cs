using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Text.Json;
using Communication;
using DataCommon;


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
        Request newClientRequest = new Request("NewClient", JsonSerializer.Serialize(newClientSubrequest));
        InvokeMessgeRecieved(this, newClientRequest);
    }
    private void SendDelClientRequest(Guid clientId)
    {
        NewClientRequest delClientSubrequest = new NewClientRequest(clientId);
        Request delClientRequest = new Request("DelClient", JsonSerializer.Serialize(delClientSubrequest));
        InvokeMessgeRecieved(this, delClientRequest);
    }

    private async Task HandleClient(HttpListenerContext context)
    {
        var wsContext = await context.AcceptWebSocketAsync(null);
        var socket = wsContext.WebSocket;
        var clientId = Guid.NewGuid();
        var buffer = new byte[4096];

        AddClientSocketPair(clientId, socket);
        SendNewClientRequest(clientId);

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
                if (result.MessageType == WebSocketMessageType.Close) break;
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Request request;
                try
                {
                    request = JsonSerializer.Deserialize<Request>(msg);
                }
                catch
                {
                    Console.WriteLine($"Error deserializing message: {msg}");
                    continue;
                }
                if (request == null) continue;

                InvokeMessgeRecieved(this, request);
            }
        }
        finally
        {
            RemoveClientSocketPair(clientId, socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            socket.Dispose();

            SendDelClientRequest(clientId);

            //Console.WriteLine($"Client disconnected: {clientId}");
        }
    }

    public void ServerLoop()
    {
        var serverLoopTask = Task.Run(() => ServerLoopImplAsync());
        serverLoopTask.Wait();
    }
    private async Task ServerLoopImplAsync()
    {
        Console.WriteLine("Server loop started.");
        while (true)
        {
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
            Console.WriteLine($"Broadcasting to {clientsSockets.Count()}");
            foreach (var clientId in clientsSockets.Keys)
            {
                Console.WriteLine($"Broadcasting to {clientId.ToString()}");
                SendMessage(clientId, request);
            }
        }
        return Task.CompletedTask;
    }
    public Task SendMessage(Guid clientId, Request request)
    {
        var message = JsonSerializer.Serialize(request);
        var buffer = Encoding.UTF8.GetBytes(message);
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
