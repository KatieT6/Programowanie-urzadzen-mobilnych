using DataCommon;

namespace DataServer;

public interface IDataLayer
{
    public static IDataLayer CreateDataLayer() { return new DataLayer(); }

    public void GetAllBooks(in List<IBook> books);
    public void AddBook(IBook book);
    public void RemoveBook(IBook book);
    public void AddBooks(List<IBook> books);
    public void RemoveBooks(List<IBook> books);
    public IBook GetBookByID(Guid id);
    public bool TryMarkAsAvailable(Guid id);
    public bool TryMarkAsUnavailable(Guid id);
    public int Count();
}
