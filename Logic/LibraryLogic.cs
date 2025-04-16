using DataClient;
using DataCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicClient
{
    public class LibraryLogic : ILibraryLogic
    {
        private readonly object _lock = new object();
        private IDatabase library;
        public LibraryLogic(IDatabase library = default)
        {
            this.library = library;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddBook(IBook book)
        {
            library.AddBook(book);
            OnPropertyChanged(nameof(book));
        }

        public void Clear()
        {
            lock (_lock)
            {
                library.Shelf.Clear();
            }
        }

        public List<IBook> GetAllBooks()
        {
            lock (_lock)
            {
                return library.Shelf;
            }
        }

        public IBook GetBookByID(Guid id)
        {
            lock (_lock)
            {
                var book = library.Shelf.FirstOrDefault(b => b.Id == id);
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
                return library.GetBooksByID(ids);
            }
        }

        public List<IBook> GetBooksByType(BookType type)
        {
            lock (_lock)
            {
                return library.GetBooksByType(type);
            }
        }

        public void LendBook(IBook book)
        {
            lock (_lock)
            {
                if (book.IsAvailable)
                {
                    library.MarkBookAsUnavailable(book);
                }
                /*else
                {
                    throw new InvalidOperationException("Book is already lent out.");
                }*/
            }
        }

        public void LendBookByID(Guid id)
        {
            lock (_lock)
            {
                var book = library.Shelf.FirstOrDefault(b => b.Id == id);
                if (book != null)
                {
                    LendBook(book);

                    OnPropertyChanged(nameof(book));
                }
                /*else
                {
                    throw new KeyNotFoundException("Book not found.");
                }*/
            }
        }

        public void ReturnBook(IBook book)
        {
            lock (_lock) 
            {
                if (!book.IsAvailable)
                {
                    library.MarkBookAsAvailable(book);
                    OnPropertyChanged(nameof(book));
                }
                else
                {
                    throw new InvalidOperationException("Book is already returned.");
                }
            }
        }

        public void ReturnBookByID(Guid id)
        {
            lock (_lock)
            {
                var book = library.Shelf.FirstOrDefault(b => b.Id == id);
                if (book != null)
                {
                    ReturnBook(book);
                }
                else
                {
                    throw new KeyNotFoundException("Book not found.");
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
