using System.ComponentModel;

namespace DataCommon;

internal class Book : IBook
{
    private string title = "Default Title";
    private string author = "Author Title";
    private int year = 0;
    private BookType type = BookType.None;
    private bool isAvailable = true;
    private Guid id = Guid.NewGuid();

    public string Title { get => title; set => title = value; }
    public string Author { get => author; set => author = value; }
    public int Year { get => year; set => year = value; }
    public BookType Type { get => type; set => type = value; }
    public bool IsAvailable { get => isAvailable; set => isAvailable = value; }
    public Guid Id { get => id; set => id = value; }

    public Book(IBookInitData init)
    {
        Title = init.Title;
        Author = init.Author;
        Year = init.Year;
        Type = init.Type;

        IsAvailable = true;
        Id = Guid.NewGuid();
    }

    public Book(string title, string author, int year, BookType type, Guid id, bool isAvailable) 
    {
        Title = title;
        Author = author;
        Year = year;
        Type = type;
        Id = id;
        IsAvailable = isAvailable;
    }

    public Book(string title, string author, int year, BookType type)
    {
        Title = title;
        Author = author;
        Year = year;
        Type = type;
        Id = Guid.NewGuid();
        IsAvailable = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
