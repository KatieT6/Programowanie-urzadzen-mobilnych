
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;


namespace Client;

public class WSClient
{
    private string uri_;
    private ConcurrentQueue<string> messageQueue_;
    private SemaphoreSlim signal_;

    public WSClient (string uri, ConcurrentQueue<string> queue, SemaphoreSlim signal)
    {
        uri_ = uri;
        messageQueue_ = queue;
        signal_ = signal;
    }

    public async Task Start()
    {
        using var socket = new ClientWebSocket();
        await socket.ConnectAsync(new Uri(uri_), CancellationToken.None);
        Console.WriteLine("Connected to server.");

        _ = ReceiveLoop(socket);

        while (true)
        {
            if (messageQueue_.IsEmpty)
            {
                await signal_.WaitAsync();
                if (messageQueue_.TryDequeue(out var message))
                {
                    Console.WriteLine($"Sending: {message}");
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    await Task.Delay(10);
                }
            }   
        }

        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
    }

    private async Task ReceiveLoop(ClientWebSocket socket)
    {
        var buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received: {msg}");
        }
    }
}
