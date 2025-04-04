﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal class Library : ILibrary
    {
        public Library() 
        {
            Shelf = new();
            AddBook(new Book("title1", "author1", 2137, BookType.SciFi));
            AddBook(new Book("title2", "author2", 2137, BookType.SciFi));
            AddBook(new Book("title3", "author3", 2137, BookType.SciFi));
            AddBook(new Book("title4", "author4", 2137, BookType.SciFi));
            AddBook(new Book("title5", "author5", 2137, BookType.SciFi));
            AddBook(new Book("title6", "author6", 2137, BookType.SciFi));
            AddBook(new Book("title7", "author7", 2137, BookType.SciFi));

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
