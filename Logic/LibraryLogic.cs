using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class LibraryLogic : ILibraryLogic
    {
        private readonly object _lock = new object();
        private ILibrary library;

        public LibraryLogic(ILibrary library = default)
        {
            this.library = library;
        }

        public void AddBook(IBook book)
        {
            library.AddBook(book);
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
                else
                {
                    throw new InvalidOperationException("Book is already lent out.");
                }
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
                }
                else
                {
                    throw new KeyNotFoundException("Book not found.");
                }
            }
        }

        public void ReturnBook(IBook book)
        {
            lock (_lock) 
            {
                if (!book.IsAvailable)
                {
                    library.MarkBookAsAvailable(book);
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
    }
}
