using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public struct BookInit
    {
        public string title;
        public string author;
        public int year;
        public BookType type;

        public BookInit(string title, string author, int year, BookType type)
        {
            this.title = title;
            this.author = author;
            this.year = year;
            this.type = type;
        }
    }

    internal class Book : IBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }

        public BookType Type { get; set; }
        public bool IsAvailable { get; set; }

        public Guid Id { get; set; }

        public Book(BookInit init)
        {
            Id = Guid.NewGuid();
            Title = init.title;
            Author = init.author;
            Year = init.year;
            Type = init.type;
            IsAvailable = true;
        }

        public Book(string title, string author, int year, BookType type)
        {
            Id = Guid.NewGuid();
            Title = title;
            Author = author;
            Year = year;
            Type = type;
            IsAvailable = true;
        }

        public Book() 
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            Author = string.Empty;
            Year = 0;
            Type = BookType.NonFiction;
            IsAvailable = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public enum BookType
    {
        SciFi,
        Fantasy,
        Romance,
        Horror,
        Thriller,
        Mystery,
        NonFiction,
        Historical,
        Biography,
        Poetry
    }
}
