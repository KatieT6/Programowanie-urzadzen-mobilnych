
using Data;
using PresentationModel;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;


namespace Client;

public class WSClient : IObserver<IBook>
{
    private string uri_;
    private ConcurrentQueue<string> messageQueue_;
    private SemaphoreSlim signal_;
    private ModelLibrary library_;
    private ObservableCollection<ModelBook> books_;
    private ObservableCollection<ModelBook> borrowedBooks_;

    private IDisposable? unsubscriber_;

    public WSClient (string uri, ConcurrentQueue<string> queue, SemaphoreSlim signal, ModelLibrary library, ObservableCollection<ModelBook> books, ObservableCollection<ModelBook> borrowedBooks)
    {
        uri_ = uri;
        messageQueue_ = queue;
        signal_ = signal;
        library_ = library;
        books_ = books;
        borrowedBooks_ = borrowedBooks;
    }
    public void Subscribe(IObservable<IBook> provider)
    {
        if (provider != null)
        {
            unsubscriber_ = provider.Subscribe(this);
        }
    }

    public void OnCompleted()
    {
        Console.WriteLine("No more books to receive.");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"Error: {error.Message}");
    }

    public void OnNext(IBook value)
    {
        Console.WriteLine($"Received book: {value.Title} by {value.Author}");
    }

    public void Unsubscribe()
    {
        unsubscriber_?.Dispose();
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
