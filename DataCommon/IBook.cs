using System.ComponentModel;

namespace DataCommon;

public interface IBook : INotifyPropertyChanged
{
    string Title { get; set; }
    string Author { get; set; }
    int Year { get; set; }
    BookType Type { get; set; }
    Guid Id { get; set; }
    bool IsAvailable { get; set; }

    public static IBook CreateBook(IBookInitData init) { return new Book(init); }
    public static IBook CreateBook(string title, string author, int year, BookType type, Guid id, bool isAvailable)
    {
        return new Book(title, author, year, type, id, isAvailable);
    }

    public static IBook CreateBook(string title, string author, int year, BookType type)
    {
        return new Book(title, author, year, type);
    }
}
