using DataClient;
using DataCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Client;
using Communication;
using System.Text.Json;
using System.Xml.Serialization;

namespace LogicClient
{
    public class LibraryLogic : ILibraryLogic, IObserver<Request>
    {
        private readonly object _lock = new object();
        private IDatabase library;
        private IClient Client;

        public IDatabase Library { get => library; set => library = value; }

        public LibraryLogic(IDatabase library, IClient client = default)
        {
            this.Library = library;
            Client = client;

            /*if (Client != null)
            {
                Client.messageRecieved += OnMessageReceived!;
            }*/

            if (Client is IObservable<Request> observable)
            {
                _subscription = observable.Subscribe(this);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private IDisposable? _subscription;


        public void AddBook(IBook book)
        {
            Library.AddBook(book);
            OnPropertyChanged(nameof(book));
        }

        public void Clear()
        {
            lock (_lock)
            {
                Library.Shelf.Clear();
            }
        }

        public List<IBook> GetAllBooks()
        {
            lock (_lock)
            {
                return Library.Shelf;
            }
        }

        public IBook GetBookByID(Guid id)
        {
            lock (_lock)
            {
                var book = Library.Shelf.FirstOrDefault(b => b.Id == id);
                if (book != null)
                {
                    return book;
                }
                else
                {
                    throw new KeyNotFoundException("Book not found.");
                }
            }
        }

        public List<IBook> GetBooksByID(List<Guid> ids)
        {
            lock (_lock)
            {
                return Library.GetBooksByID(ids);
            }
        }

        public List<IBook> GetBooksByType(BookType type)
        {
            lock (_lock)
            {
                return Library.GetBooksByType(type);
            }
        }

        public void LendBook(IBook book)
        {
            lock (_lock)
            {
                //Console.WriteLine("Client ID: " + Client.ClientId);
                var borrowRequest = new ReturnBorrowRequest(Client.ClientId, book.Id);

                string xmlMessage = "";
                var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowRequest));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, borrowRequest);
                    xmlMessage = stringWriter.ToString();
                }

                var request = new Request("BorrowBook", xmlMessage);
                Client.SendMessage(request);
            }
        }

        public void LendBookByID(Guid id)
        {
            lock (_lock)
            {
                 var borrowRequest = new ReturnBorrowRequest(Client.ClientId, id);

                string xmlMessage = "";
                var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowRequest));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, borrowRequest);
                    xmlMessage = stringWriter.ToString();
                }

                var request = new Request("BorrowBook", xmlMessage);
                 Client.SendMessage(request);
            }
        }

        public void ReturnBook(IBook book)
        {
            lock (_lock) 
            {
                var returnRequest = new ReturnBorrowRequest(Client.ClientId, book.Id);

                string xmlMessage = "";
                var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowRequest));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, returnRequest);
                    xmlMessage = stringWriter.ToString();
                }

                var request = new Request("ReturnBook", xmlMessage);
                Client.SendMessage(request);
            }
        }

        public void ReturnBookByID(Guid id)
        {
            lock (_lock)
            {
                var returnRequest = new ReturnBorrowRequest(Client.ClientId, id);

                string xmlMessage = "";
                var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowRequest));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, returnRequest);
                    xmlMessage = stringWriter.ToString();
                }

                var request = new Request("ReturnBook", xmlMessage);
                Client.SendMessage(request);
            }
        }

        public void LoadRequest()
        {
            lock (_lock)
            {
                var loadRequest = new LoadRequest();

                string xmlMessage = "";
                var xmlSerializer = new XmlSerializer(typeof(LoadRequest));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, loadRequest);
                    xmlMessage = stringWriter.ToString();
                }

                var request = new Request("LoadRequest", xmlMessage);
                Client.SendMessage(request);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private void HandleBorrowBookRequest(Request inrequest)
        {
            ReturnBorrowResponseRequest request;
            var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowResponseRequest));
            using (var stringReader = new StringReader(inrequest.ArgsJson))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not ReturnBorrowResponseRequest deserializedRequest) return;
                request = (ReturnBorrowResponseRequest)deserialized;
            }

            if (request != null && request.Result == 1)
            {
                lock (_lock)
                {
                    var book = Library.Shelf.FirstOrDefault(b => b.Id == request.BookId);
                    if (book != null && book.IsAvailable)
                    {
                        Library.MarkBookAsUnavailable(book);
                        OnPropertyChanged(nameof(book));
                    }
                }
            }
        }

        private void HandleReturnBookRequest(Request inrequest)
        {
            ReturnBorrowResponseRequest request;
            var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowResponseRequest));
            using (var stringReader = new StringReader(inrequest.ArgsJson))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not ReturnBorrowResponseRequest deserializedRequest) return;
                request = (ReturnBorrowResponseRequest)deserialized;
            }

            if (request != null)
            {
                lock (_lock)
                {
                    var book = Library.Shelf.FirstOrDefault(b => b.Id == request.BookId);
                    if (book != null && !book.IsAvailable)
                    {
                        Library.MarkBookAsAvailable(book);
                        OnPropertyChanged(nameof(book));
                    }
                }
            }
        }
        
        private void HandleLoadRequest(Request inrequest)
        {
            LoadRequest request;
            var xmlSerializer = new XmlSerializer(typeof(LoadRequest));
            using (var stringReader = new StringReader(inrequest.ArgsJson))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not LoadRequest deserializedRequest) return;
                request = (LoadRequest)deserialized;
            }

            if (request != null)
            {
                lock (_lock)
                {
                    Library.Shelf.Clear();
                    foreach (var book in request.Books)
                    {
                        Library.AddBook(IBook.CreateBook(book.Title, book.Author, book.Year, book.Type, book.Id, book.IsAvailable));
                        
                    }
                }
            }
            OnPropertyChanged(nameof(Library));
        }

        /*private void OnMessageReceived(object sender, Request request)
        {
            switch (request.Name)
            {
                case "BorrowBook":
                    HandleBorrowBookRequest(request);
                    break;
                case "ReturnBook":
                    HandleReturnBookRequest(request);
                    break;
                case "LoadRequest":
                    HandleLoadRequest(request);
                    break;
                default:
                    break;
            }
        }*/

        public void OnNext(Request request)
        {
            switch (request.Name)
            {
                case "BorrowBook":
                    HandleBorrowBookRequest(request);
                    break;
                case "ReturnBook":
                    HandleReturnBookRequest(request);
                    break;
                case "LoadRequest":
                    HandleLoadRequest(request);
                    break;
            }
        }

        public void OnCompleted()
        {
            Console.WriteLine("WebSocket zakończył transmisję.");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"Błąd WebSocket: {error.Message}");
        }
    }
}
