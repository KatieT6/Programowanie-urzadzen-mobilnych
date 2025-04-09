using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IBook : INotifyPropertyChanged
    {
        string Title { get; set; }
        string Author { get; set; }
        int Year { get; set; }
        BookType Type { get; set; }
        Guid Id { get; set; }
        bool IsAvailable { get; set; }

        public static IBook CreateBook() { return new Book(); }
        public static IBook CreateBook(string title, string author, int year, BookType type) { return new Book(title, author, year, type); }
        public static IBook CreateBook(BookInit init) { return new Book(init); }

        public static IBook CreateBook(string title, string author, int year, BookType type, Guid id, bool isAvailable)
        {
            return new Book(title, author, year, type, id, isAvailable);
        }
    }
}
