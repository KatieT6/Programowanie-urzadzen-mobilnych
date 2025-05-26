using DataCommon;

namespace DataServer;

internal class DataLayer : IDataLayer
{
    private IDatabase database;

    public DataLayer()
    {
        database = IDatabase.CreateDatabase();
    }

    public void GetAllBooks(in List<IBook> books)
    {
        database.GetAllBooks(books);
    }
    public void AddBook(IBook book)
    {
        database.AddBook(book);
    }
    public void RemoveBook(IBook book)
    {
        database.RemoveBook(book);
    }
    public void AddBooks(List<IBook> books)
    {
        database.AddBooks(books);
    }
    public void RemoveBooks(List<IBook> books)
    {
        database.RemoveBooks(books);
    }
    public IBook GetBookByID(Guid id)
    {
        return database.GetBookByID(id);
    }
    public bool TryMarkAsAvailable(Guid id)
    {
        return database.TryMarkAsAvailable(id);
    }
    public bool TryMarkAsUnavailable(Guid id)
    {
        return database.TryMarkAsUnavailable(id);
    }
    public int Count()
    {
        return database.Count();
    }
}
