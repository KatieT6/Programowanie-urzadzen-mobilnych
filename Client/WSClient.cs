
using System.Net.WebSockets;
using System.Text;


namespace Client;

internal class WSClient
{
    public int currentResourceValue = 0;
    

    public async Task Start(string uri)
    {
        using var socket = new ClientWebSocket();
        await socket.ConnectAsync(new Uri(uri), CancellationToken.None);
        Console.WriteLine("Connected to server.");

        _ = ReceiveLoop(socket);

        while (true)
        {
            var msg = Console.ReadLine();

            if (msg == "exit" || msg == null) break;

            for (int i = 0; i < 1000; i++)
            {
                var buffer = Encoding.UTF8.GetBytes(msg);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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
