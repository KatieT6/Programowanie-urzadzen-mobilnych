namespace DataCommon;

public class IBookInitData
{
    private string title = "Default Title";
    private string author = "Author Title";
    private int year = 0;
    private BookType type = BookType.None;

    public string Title { get => title; set => title = value; }
    public string Author { get => author; set => author = value; }
    public int Year { get => year; set => year = value; }
    public BookType Type { get => type; set => type = value; }

    public IBookInitData(string title, string author, int year, BookType type) 
    {
        this.title = title;
        this.author = author;
        this.year = year;
        this.type = type;
    }
}
