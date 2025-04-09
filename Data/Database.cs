using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal class Database : IDatabase
    {
        public Database() 
        {
            Shelf = new();
        }
        public List<IBook> Shelf { get; set; }

        public void AddBook(IBook book)
        {
            if (!Shelf.Any(b => b.Id == book.Id))
            {
                Shelf.Add(book);
            }
        }

        public IBook GetBookByID(Guid id)
        {
            var book = Shelf.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }
            else
            {
                return book;
            }
        }

        public void RemoveBook(IBook book)
        {
            var toBeRemoved = Shelf.Find(b => b.Id == book.Id);
            if (toBeRemoved == null) return;
            Shelf.Remove(toBeRemoved);
        }

        public void AddBooks(List<IBook> books)
        {
            foreach (var book in books)
            {
                AddBook(book);
            }
        }

        public List<IBook> GetBooksByID(List<Guid> ids)
        {
            return Shelf.FindAll(b => ids.Contains(b.Id));
        }

        public void RemoveBooks(List<IBook> books)
        {
            foreach (var book in books)
            {
                RemoveBook(book);
            }
        }
        
        public List<IBook> GetBooksByType(BookType type)
        {
            return Shelf.FindAll(b => b.Type == type);
        }

        public void MarkBookAsAvailable(IBook book)
        {
            var bookToUpdate = Shelf.FirstOrDefault(b => b.Id == book.Id);
            if (bookToUpdate != null)
            {
                bookToUpdate.IsAvailable = true;
            }
        }

        public void MarkBookAsUnavailable(IBook book)
        {
            var bookToUpdate = Shelf.FirstOrDefault(b => b.Id == book.Id);
            if (bookToUpdate != null)
            {
                bookToUpdate.IsAvailable = false;
            }
        }

        public List<IBook> GetAllBooks()
        {
            return Shelf;
        }

        
    }
    
}
