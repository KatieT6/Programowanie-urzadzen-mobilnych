using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal class Book : IBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }

        public BookType Type { get; set; }
        public bool IsAvailable { get; set; }

        public Guid Id { get; set; }

        public Book(string title, string author, int year, BookType type)
        {
            Id = Guid.NewGuid();
            Title = title;
            Author = author;
            Year = year;
            Type = type;
            IsAvailable = true;
        }
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
