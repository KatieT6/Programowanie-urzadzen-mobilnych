using Logic;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using Data;
using System.Text.Json;

namespace Server;

internal class Server : IServer
{
    private ILogicLayer logicLayer;

    private HttpListener listener_ = new();
    private ConcurrentDictionary<string, WebSocket> clients_ = new();

    private ConcurrentQueue<(WebSocket Socket, string Message)> queue_ = new();

    private Queue<BookInit> publisherQueue = new();

    private SemaphoreSlim signal_ = new(0);
    
    public Server() 
    {
        logicLayer = ILogicLayer.CreateLogicLayer();
    }

    private Task SendToSingleClientAsync(WebSocket socket, string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);
        return socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task SendToAllClientsAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);
        foreach (var client in clients_.Values)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    private async Task HandleRequest(WebSocket socket, string message)
    {
        await Task.Delay(1);
    }
    
    private async Task ProcessMessageQueueAsync()
    {
        while (true)
        {
            await signal_.WaitAsync();

            while (queue_.TryDequeue(out var entry))
            {
                var (socket, message) = entry;

                new Thread(() =>
                {
                    HandleRequest(socket, message).GetAwaiter().GetResult();
                })
                { IsBackground = true }.Start();
            }
        }
    }

    private async Task PublisherLoopAsync()
    {
        while (true)
        {
            await Task.Delay(20000);
            if (publisherQueue.Count > 0)
            {
                var bookInit = publisherQueue.Dequeue();
                logicLayer.LibraryLogic.AddBook(IBook.CreateBook(bookInit));
                var message = $"New book added: {bookInit.title} by {bookInit.author} ({bookInit.year})";
                await SendToAllClientsAsync(message);
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine("No new books to publish.");
            }
        }
    }

    private async Task HandleClient(HttpListenerContext context)
    {
        var wsContext = await context.AcceptWebSocketAsync(null);
        var socket = wsContext.WebSocket;
        var clientId = Guid.NewGuid().ToString();

        clients_[clientId] = socket;

        var buffer = new byte[4096];
        Console.WriteLine($"Client connected: {clientId}");

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close) break;
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received from {clientId}: {msg}");

                queue_.Enqueue((socket, msg));
                signal_.Release();
            }
        }
        finally
        {
            clients_.TryRemove(clientId, out _);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            socket.Dispose();
            Console.WriteLine($"Client disconnected: {clientId}");
        }
    }

    private async Task ServerLoopImpl()
    {
        Console.WriteLine("Server loop started.");
        while (true)
        {
            var context = await listener_.GetContextAsync();
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
    
    public void Init()
    {
        Console.WriteLine("Server is starting up...");
        
        listener_.Prefixes.Add("http://localhost:5000/ws/");
        listener_.Start();

        _ = Task.Run(PublisherLoopAsync);
        _ = Task.Run(ProcessMessageQueueAsync);

        Console.WriteLine("Server is up and running.");
    }

    public void ServerLoop()
    {
        Console.WriteLine("Server is running...");
        var serverLoopTask = Task.Run(() => ServerLoopImpl());
        serverLoopTask.Wait();
    }

    public void LendBook(Guid id)
    {
        logicLayer.LibraryLogic.LendBookByID(id);
    }
    
    public void ReturnBook(Guid id)
    {
        logicLayer.LibraryLogic.ReturnBookByID(id);
    }
}
