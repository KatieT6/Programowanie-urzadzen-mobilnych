﻿using DataClient;
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

namespace LogicClient
{
    public class LibraryLogic : ILibraryLogic
    {
        private readonly object _lock = new object();
        private IDatabase library;
        private IClient Client;

        public IDatabase Library { get => library; set => library = value; }

        public LibraryLogic(IDatabase library, IClient client = default)
        {
            this.Library = library;
            Client = client;
            if (Client != null)
            {
                Client.messageRecieved += OnMessageReceived!;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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
                var borrowRequest = new ReturnBorrowRequest(Client.ClientId, book.Id);
                var request = new Request("BorrowBook", JsonSerializer.Serialize(borrowRequest));
                Client.SendMessage(request);
            }
        }

        public void LendBookByID(Guid id)
        {
            lock (_lock)
            {
                 var borrowRequest = new ReturnBorrowRequest(Client.ClientId, id);
                 var request = new Request("BorrowBook", JsonSerializer.Serialize(borrowRequest));
                 Client.SendMessage(request);
            }
        }

        public void ReturnBook(IBook book)
        {
            lock (_lock) 
            {
                var returnRequest = new ReturnBorrowRequest(Client.ClientId, book.Id);
                var request = new Request("ReturnBook", JsonSerializer.Serialize(returnRequest));
                Client.SendMessage(request);
            }
        }

        public void ReturnBookByID(Guid id)
        {
            lock (_lock)
            {
                var returnRequest = new ReturnBorrowRequest(Client.ClientId, id);
                var request = new Request("ReturnBook", JsonSerializer.Serialize(returnRequest));
                Client.SendMessage(request);
            }
        }

        public void LoadRequest()
        {
            lock (_lock)
            {
                var loadRequest = new LoadRequest();
                var request = new Request("LoadRequest", JsonSerializer.Serialize(loadRequest));
                Client.SendMessage(request);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnMessageReceived(object sender, Request request)
        {
            var response = JsonSerializer.Deserialize<ReturnBorrowResponseRequest>(request.ArgsJson);
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
        }
        private void HandleBorrowBookRequest(Request request)
        {
            var borrowRequest = JsonSerializer.Deserialize<ReturnBorrowResponseRequest>(request.ArgsJson);
            if (borrowRequest != null && borrowRequest.Result == 1)
            {
                lock (_lock)
                {
                    var book = Library.Shelf.FirstOrDefault(b => b.Id == borrowRequest.BookId);
                    if (book != null && book.IsAvailable)
                    {
                        Library.MarkBookAsUnavailable(book);
                        OnPropertyChanged(nameof(book));
                    }
                }
            }
        }

        private void HandleReturnBookRequest(Request request)
        {
            var returnRequest = JsonSerializer.Deserialize<ReturnBorrowRequest>(request.ArgsJson);
            if (returnRequest != null)
            {
                lock (_lock)
                {
                    var book = Library.Shelf.FirstOrDefault(b => b.Id == returnRequest.BookId);
                    if (book != null && !book.IsAvailable)
                    {
                        Library.MarkBookAsAvailable(book);
                        OnPropertyChanged(nameof(book));
                    }
                }
            }
        }
        private void HandleLoadRequest(Request request)
        {
            
            var loadRequest = JsonSerializer.Deserialize<LoadRequest>(request.ArgsJson);
            if (loadRequest != null)
            {
                lock (_lock)
                {
                    Library.Shelf.Clear();
                    foreach (var book in loadRequest.Books)
                    {
                        Library.AddBook(IBook.CreateBook(book.Title, book.Author, book.Year, book.Type, book.Id, book.IsAvailable));
                        
                    }
                }
            }
            OnPropertyChanged(nameof(Library));
        }

    }
}
