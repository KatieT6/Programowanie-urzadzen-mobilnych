using DataCommon;
using System.Text;

namespace LogicServer;

internal class Publisher : IPublisher
{
    private readonly string bookListFile = "book_list.json";
    Queue<IBookInitData> books = new Queue<IBookInitData>();
    public Publisher()
    {
        var buffer = File.ReadAllBytes(bookListFile);
        var json = Encoding.UTF8.GetString(buffer, 0, buffer.Count());
        var bookList = System.Text.Json.JsonSerializer.Deserialize<List<IBookInitData>>(json);
        if (bookList != null)
        {
            foreach (var book in bookList)
            {
                books.Enqueue(book);
            }
        }
    }

    public IBookInitData GetNewBook()
    {
        return books.Dequeue();
    }
}
