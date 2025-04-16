using DataCommon;
using System.ComponentModel;

namespace DataServer;

public interface IDatabase
{
    public event EventHandler databaseChanged;

    public void GetAllBooks(in List<IBook> books);

    public void AddBook(IBook book);
    public void RemoveBook(IBook book);
    public void AddBooks(List<IBook> books);
    public void RemoveBooks(List<IBook> books);

    public IBook GetBookByID(Guid id);

    public bool TryMarkAsAvailable(Guid id);
    public bool TryMarkAsUnavailable(Guid id);

    public int Count();

    public static IDatabase CreateDatabase() { return new Database(); }
}
