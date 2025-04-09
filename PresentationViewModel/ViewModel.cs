using PresentationModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using Communication;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections;
using Data;

namespace PresentationViewModel
{
    public class ViewModel : ViewModelBase
    {
        private Queue<string> _outMessageQueue = new Queue<string>();
        private static object _outMessageQueueLock = new();

        private Queue<string> _inMessageQueue = new Queue<string>();
        private static object _inMessageQueueLock = new();

        private SemaphoreSlim _signal = new(0);

        private readonly ModelAbstractApi _modelAPI;
        private ModelLibrary _library;
        private ObservableCollection<ModelBook> _books;
        private ObservableCollection<ModelBook> _borrowedBooks;

        private Timer _timer;

        public ModelAbstractApi ModelAPI
        {
            get { return _modelAPI; }
        }

        public ModelLibrary Library
        {
            get { return _library; }
            set
            {
                if (_library != value)
                {
                    _library = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ModelBook> Books
        {
            get { return _books; }
            set
            {
                if (_books != value)
                {
                    _books = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ModelBook> BorrowedBooks
        {
            get { return _borrowedBooks; }
            set
            {
                if (_borrowedBooks != value)
                {
                    _borrowedBooks = value;
                    OnPropertyChanged();
                }
            }
        }

        public Timer Timer
        {
            get { return _timer; }
            set
            {
                if (_timer != value)
                {
                    _timer = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool booksChanged = false;
        private object _booksChangedLock = new();

        private async Task ProcessMessageQueueAsync()
        {
            while (true)
            {
                lock(_inMessageQueueLock)
                {
                    if(_inMessageQueue.TryDequeue(out var entry))
                    {
                        Request? request = JsonSerializer.Deserialize<Request>(entry);
                        if (request != null)
                        {
                            Console.WriteLine($"Recieved: {request.Name}");

                            if(request.Name == RequestTypes.LOAD_REPLY)
                            {
                                LoadReply(request.Args);
                            }
                            else if(request.Name == RequestTypes.BORROW_REPLY)
                            {
                                BorrowReply(request.Args);
                            }
                            else if(request.Name == RequestTypes.RETURN_REPLY)
                            {
                                ReturnReply(request.Args);
                            }
                            else
                            {
                                Console.WriteLine($"Unknown request type: {request.Name}");
                            }
                        }
                    }
                }
                await Task.Delay(50);
            }
        }

        private async Task ReceiveLoop(ClientWebSocket socket)
        {
            var buffer = new byte[4096];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                lock(_inMessageQueueLock)
                {
                    if(msg != "") _inMessageQueue.Enqueue(msg);
                }
            }
        }

        private async Task BookLoaderLoop()
        {
            while (true)
            {
                await Task.Delay(1000);
                lock (_booksChangedLock)
                {
                    if (booksChanged)
                    {
                        try
                        {
                            booksChanged = false;
                            var bks = _library.GetBooks();
                            _books.Clear();
                            foreach (ModelBook book in bks)
                            {
                                _books.Add(book);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error loading books: {e.Message}");
                        }

                    }
                }
            }
        }

        public async Task ClientStart()
        {
            using var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);
            Console.WriteLine("Connected to server.");

            _ = ReceiveLoop(socket);
            _ = ProcessMessageQueueAsync();

            LoadLibrary();

            //_ = PrintQueue();
            while (true)
            {
                lock(_outMessageQueueLock)
                {
                    if (_outMessageQueue.Count > 0)
                    {
                        if (_outMessageQueue.TryDequeue(out var message))
                        {
                            _signal.Release();
                            Console.WriteLine($"Sending: {message}");
                            var buffer = Encoding.UTF8.GetBytes(message);
                            _ = socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        Task.Delay(10);
                    }
                }
                
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }

        public ViewModel()
        {
            _modelAPI = ModelAbstractApi.CreateModelAPI();
            _library = _modelAPI.Library;

            _books = new ObservableCollection<ModelBook>();
            _borrowedBooks = new ObservableCollection<ModelBook>();

            //Load books from server

            //_modelAPI.Library.LoadBooks();
            //foreach (ModelBook book in _library.GetBooks())
            //{
            //    _books.Add(book);
            //}

            new Thread(() =>
            {
                _ = ClientStart();
            }).Start();

            _ = BookLoaderLoop();

            BorrowClick = new RelayCommand(param => BorrowClickHandler(param as ModelBook));
            ReturnClick = new RelayCommand(param => ReturnClickHandler(param as ModelBook));
        }

        public ICommand BorrowClick { get; set; }
        public ICommand ReturnClick { get; set; }

  
        private void BorrowClickHandler(ModelBook selectedBook)
        {
            if (selectedBook != null && selectedBook.IsAvailable)
            {
                Console.WriteLine("BorrowClickHandler");
                lock (_outMessageQueueLock)
                {
                    Request request = new Request(RequestTypes.BORROW, new List<string>() { selectedBook.Id.ToString()});
                    _outMessageQueue.Enqueue(JsonSerializer.Serialize(request));
                }

                _library.LendBook(selectedBook);
                //RefreshLibrary();
                _borrowedBooks.Add(selectedBook);
                OnPropertyChanged(nameof(Books));
                OnPropertyChanged(nameof(BorrowedBooks));
            }
        }

        private void ReturnClickHandler(ModelBook selectedBook)
        {
            if (selectedBook != null && !selectedBook.IsAvailable)
            {
                Console.WriteLine("ReturnClickHandler");
                lock (_outMessageQueueLock)
                {
                    Request request = new Request(RequestTypes.RETURN, new List<string>() { selectedBook.Id.ToString() });
                    _outMessageQueue.Enqueue(JsonSerializer.Serialize(request));
                }
                
                _signal.Release();
                selectedBook.IsAvailable = true;
                _library.ReturnBook(selectedBook);
                //RefreshLibrary();
                _borrowedBooks.Remove(selectedBook);
                OnPropertyChanged(nameof(Books));
                OnPropertyChanged(nameof(BorrowedBooks));
            }
        }

        private void LoadLibrary()
        {
            lock(_outMessageQueueLock)
            {
                Request request = new Request(RequestTypes.LOAD, new List<string>());
                _outMessageQueue.Enqueue(JsonSerializer.Serialize(request));
            }
        }


        private void BorrowReply(List<string> args)
        {
            var selectedBook = _modelAPI.Library.GetBookByID(Guid.Parse(args[0]));

            if(selectedBook == null) return;

            _borrowedBooks.Add(selectedBook);

            OnPropertyChanged(nameof(BorrowedBooks));
        }

        private void ReturnReply(List<string> args)
        {
            var selectedBook = BorrowedBooks.FirstOrDefault(b => b.Id == Guid.Parse(args[0]));
            if (selectedBook == null) return;
            _borrowedBooks.Remove(selectedBook);
            OnPropertyChanged(nameof(BorrowedBooks));
        }

        private void LoadReply(List<string> args)
        {
            var arg = args[0];
            Console.WriteLine(arg);

            List<ModelBook>? loadedBooks = JsonSerializer.Deserialize<List<ModelBook>>(arg); ;
            
            //Console.WriteLine($"loadedBooks == null {loadedBooks == null}"); 

            if (loadedBooks == null) return;
            _library.DeepClear();
            //Console.WriteLine($"loaded books: {loadedBooks.Count}");

            foreach (var book in loadedBooks)
            {
                _library.DeepAdd(book);
            }

            _modelAPI.Library.LoadBooks();
            lock (_booksChangedLock)
            {
                booksChanged = true;
            }

            OnPropertyChanged(nameof(Books));
        }
    }
}

