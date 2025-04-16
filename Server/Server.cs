using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using DataClient;
using System.Text.Json;
using Communication;
using PresentationModel;
using DataCommon;
using LogicClient;
namespace Server;

internal class Server : IServer, IObservable<IBook>
{
    private ILogicLayer logicLayer;
    private ModelAPI modelAPI;
    private HttpListener listener_ = new();
    private ConcurrentDictionary<string, WebSocket> clients_ = new();

    private ConcurrentQueue<(WebSocket Socket, string Message)> queue_ = new();

    private Queue<IBookInitData> publisherQueue = new();

    private SemaphoreSlim queueSignal_ = new(0);

    private ConcurrentDictionary<RequestTypes, Func<WebSocket, List<string>, Task>> mapping = new();

    private SemaphoreSlim mappingSignal_ = new(0);

    public Server() 
    {
        logicLayer = ILogicLayer.CreateLogicLayer();
        modelAPI = new ModelAPI(logicLayer);
        int i = 5;
        foreach (var line in File.ReadLines("book_list.txt"))
        {
            if (i < 0) break;
            i--;
            // Skip the header
            if (line.StartsWith("TITLE,AUTHOR,YEAR,GENERE"))
                continue;

            var parts = line.Split(',');

            if (parts.Length != 4)
                continue; // Skip invalid lines

            var title = parts[0].Trim();
            var author = parts[1].Trim();
            if (!int.TryParse(parts[2].Trim(), out var year))
                continue; // Skip lines with invalid year

            if (!Enum.TryParse(parts[3].Trim(), true, out BookType type))
                continue; // Skip lines with invalid genre

            logicLayer!.LibraryLogic.AddBook(IBook.CreateBook(title, author, year, type));
        }

        mapping.TryAdd(RequestTypes.BORROW, BorrowReply);
        mapping.TryAdd(RequestTypes.RETURN, ReturnReply);
        mapping.TryAdd(RequestTypes.LOAD, LoadReply);

        publisherQueue.Enqueue(new IBookInitData("The Great Gatsby", "F. Scott Fitzgerald", 1925, BookType.Romance));
        publisherQueue.Enqueue(new IBookInitData("1984", "George Orwell", 1949, BookType.SciFi));
        publisherQueue.Enqueue(new IBookInitData("The Hobbit", "J.R.R. Tolkien", 1937, BookType.Fantasy));
        publisherQueue.Enqueue(new IBookInitData("Dune", "Frank Herbert", 1965, BookType.SciFi));
        publisherQueue.Enqueue(new IBookInitData("Pride and Prejudice", "Jane Austen", 1813, BookType.Romance));
        publisherQueue.Enqueue(new IBookInitData("The Catcher in the Rye", "J.D. Salinger", 1951, BookType.Mystery));
        publisherQueue.Enqueue(new IBookInitData("The Lord of the Rings", "J.R.R. Tolkien", 1954, BookType.Fantasy));
        publisherQueue.Enqueue(new IBookInitData("Brave New World", "Aldous Huxley", 1932, BookType.SciFi));
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

    private async Task BorrowReply(WebSocket socket, List<string> args)
    {
        Console.WriteLine($"Handling borrow request");
        var arg = args[0];
        Guid id = Guid.Parse(arg);
        
        var book = logicLayer.LibraryLogic.GetBookByID(id);
        if (book.IsAvailable)
        {
            logicLayer.LibraryLogic.LendBookByID(id);
            Request request = new Request(RequestTypes.BORROW_REPLY, new List<string>() { id.ToString() });
            SendToSingleClientAsync(socket, JsonSerializer.Serialize(request)).Wait();
            Task.Delay(250).Wait();
            foreach (var client in clients_.Values)
            {
                if (client.State == WebSocketState.Open)
                {
                    await LoadReply(client, new());
                }
            }

        }
    }

    private async Task ReturnReply(WebSocket socket, List<string> args)
    {
        Console.WriteLine($"Handling reuturn request");
        var arg = args[0];
        Guid id = Guid.Parse(arg);
        var book = logicLayer.LibraryLogic.GetBookByID(id);

        if (!book.IsAvailable)
        {
            logicLayer.LibraryLogic.ReturnBookByID(id);
            Request request = new Request(RequestTypes.RETURN_REPLY, new List<string>() { id.ToString() });
            SendToSingleClientAsync(socket, JsonSerializer.Serialize(request)).Wait();
            Task.Delay(250).Wait();
            foreach (var client in clients_.Values)
            {
                if (client.State == WebSocketState.Open)
                {
                    LoadReply(client, new()).Wait();
                }
            }

        }
    }

    private async Task LoadReply(WebSocket socket, List<string> args)
    {
        var serialized = JsonSerializer.Serialize(logicLayer.LibraryLogic.GetAllBooks());
        Request request = new Request(RequestTypes.LOAD_REPLY, new List<string>() { serialized });
        var serializedRequest = JsonSerializer.Serialize(request);
        await SendToSingleClientAsync(socket, serializedRequest);
    }

    private async Task HandleRequest(WebSocket socket, string message)
    {
        Request? request = JsonSerializer.Deserialize<Request>(message);

        Console.WriteLine($"Handling: {message}");

        if (request == null) return;

        _ = mapping[request!.Name](socket, request!.Args);

        await Task.Delay(1);
    }
    
    private async Task ProcessMessageQueueAsync()
    {
        while (true)
        {
            await queueSignal_.WaitAsync();

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
            await Task.Delay(5000);
            if (publisherQueue.Count > 0)
            {
                var bookInit = publisherQueue.Dequeue();
                logicLayer.LibraryLogic.AddBook(IBook.CreateBook(bookInit));
                var message = $"New book added: {bookInit.title} by {bookInit.author} ({bookInit.year})";
                foreach (var client in clients_.Values)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        await LoadReply(client, new());
                    }
                }
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
                queue_.Enqueue((socket, msg));
                queueSignal_.Release();
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

    public IDisposable Subscribe(IObserver<IBook> observer)
    {
        throw new NotImplementedException();
    }
}
