using Communication;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;


namespace Client;

public class WSClient : IClient
{
    private ConcurrentQueue<string> messageQueue_;
    private SemaphoreSlim signal_;
    private string uri_ = "ws://localhost:5000/ws/";
    public Guid ClientId { get; set; }

    public WSClient() 
    {
        _webSocket = new ClientWebSocket();
        ClientId = new Guid(); 
    }

    public event EventHandler<Request> messageRecieved;

    public ClientWebSocket _webSocket;


    public void ClientLoop()
    {
        var clientLoopTask = Task.Run(() => Start());
        var clientMSG = Task.Run( () =>
        { 
            Task.Delay(3000).Wait();
            _ = SendMessageAsync("Wiadomość od klienta");
        });

    }
    public async Task Start()
    {
        await _webSocket.ConnectAsync(new Uri(uri_), CancellationToken.None);
        Console.WriteLine("Connected to server.");

        _ = ReceiveLoop();

        while (true)
        {
            if (messageQueue_.IsEmpty)
            {
                await signal_.WaitAsync();
                if (messageQueue_.TryDequeue(out var message))
                {
                    Console.WriteLine($"Sending: {message}");
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    await Task.Delay(10);
                }
            }
        }

        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
    }

    public async Task SendMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[1024];
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received: {msg}");
        }
    }

    
}
