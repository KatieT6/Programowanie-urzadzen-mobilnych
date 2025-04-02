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
            return library.Shelf;
        }

        public List<IBook> GetBooksByID(List<Guid> ids)
        {
            return library.GetBooksByID(ids);
        }

        public List<IBook> GetBooksByType(BookType type)
        {
            return library.GetBooksByType(type);
        }

        public void LendBook(IBook book)
        {
            if (book.IsAvailable)
            {
                book.IsAvailable = false;
            }
            else
            {
                throw new InvalidOperationException("Book is already lent out.");
            }
        }

        public void LendBookByID(Guid id)
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

        public void ReturnBook(IBook book)
        {
            if (!book.IsAvailable)
            {
                book.IsAvailable = true;
            }
            else
            {
                throw new InvalidOperationException("Book is already returned.");
            }
        }

        public void ReturnBookByID(Guid id)
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
