using Communication;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using XMLXSDValidator;


namespace Client;

public class WSClient : IClient
{
    private Guid clientID;
    private ConcurrentQueue<string> messageQueue_ = new ConcurrentQueue<string>();
    private SemaphoreSlim signal_ = new SemaphoreSlim(0);
    private string uri_ = "ws://localhost:5000/ws/";
    public Guid ClientId { get => clientID; set { clientID = value; } }
    XSDValidator xmlXSDValidator = new XSDValidator();
    public WSClient() 
    {
        _webSocket = new ClientWebSocket();
        ClientId = new Guid(); 
    }

    public event EventHandler<Request> messageRecieved;

    public ClientWebSocket _webSocket;


    public Task SendMessage(Request request)
    {
        string xmlMessage = "";
        var xmlSerializer = new XmlSerializer(typeof(Request));
        using (var stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, request);
            xmlMessage = stringWriter.ToString();
        }

        return SendMessageAsync(xmlMessage);
    }
    public async Task SendMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public void ClientLoop()
    {
        var clientLoopTask = Task.Run(() => Start());
    }
    public async Task Start()
    {
        await _webSocket.ConnectAsync(new Uri(uri_), CancellationToken.None);
        //Console.Writeline("Connected to server.");

        _ = ReceiveLoop();

        while (true)
        {
            if (messageQueue_.IsEmpty)
            {
                await signal_.WaitAsync();
                if (messageQueue_.TryDequeue(out var message))
                {
                    //Console.Writeline($"Sending: {message}");
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    await Task.Delay(10);
                }
            }
        }

        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
    }
    private async Task ReceiveLoop()
    {
        var buffer = new byte[1024 * 100];
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            //Console.WriteLine($"Received: {msg}");

            ;
            Console.WriteLine($"Message validated: {xmlXSDValidator.Validate(msg, typeof(Request))}");

            try
            {
                Request request;
                var xmlSerializer = new XmlSerializer(typeof(Request));
                using (var stringReader = new StringReader(msg))
                {
                    var deserialized = xmlSerializer.Deserialize(stringReader);
                    if (deserialized is not Request deserializedRequest) continue;
                    request = (Request)deserialized;
                    Console.WriteLine($"Deserialized: {request.Name}");

                    if(request.Name == "NewClient")
                    {
                        NewClientRequest newClientRequest;
                        var newClientRequestXmlSerializer = new XmlSerializer(typeof(NewClientRequest));

                        using (var newClientStringReader = new StringReader(request.ArgsJson))
                        {
                            var deserializedNCR = newClientRequestXmlSerializer.Deserialize(newClientStringReader);
                            if (deserializedNCR is not NewClientRequest deserializedNCR2) continue;
                            newClientRequest = (NewClientRequest)deserializedNCR;
                        }

                        ClientId = newClientRequest.Id;
                        Console.WriteLine($"Client ID set to: {ClientId}");
                        continue;
                    }

                    messageRecieved?.Invoke(this, request);
                }
            }
            catch (Exception ex)
            {
                //Console.Writeline("Failed to deserialize message: " + ex.Message);
            }
        }
    }    
}
