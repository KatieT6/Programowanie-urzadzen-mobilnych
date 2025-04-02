using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal class Library : ILibrary
    {
        public List<IBook> Shelf { get; set; }

        public void AddBook(IBook book)
        {
            if (!Shelf.Any(b => b.Id == book.Id))
            {
                Shelf.Add(book);
            }
        }

        public void AddBooks(List<IBook> books)
        {
            foreach (var book in books)
            {
                AddBook(book);
            }
        }

        public void RemoveBook(IBook book)
        {
            if (!Shelf.Any(b => b.Id == book.Id))
            {
                Shelf.Remove(book);
            }
        }

        public void RemoveBooks(List<IBook> books)
        {
            foreach (var book in books)
            {
                RemoveBook(book);
            }
        }
        public List<IBook> GetBooksByID(List<Guid> ids)
        {
            return Shelf.FindAll(b => ids.Contains(b.Id));
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
