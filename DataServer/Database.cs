using DataCommon;

namespace DataServer;

internal sealed class Database : IDatabase
{
    private Dictionary<Guid, IBook> database = new Dictionary<Guid, IBook>();
    private object databaseLock = new object();
    public event EventHandler databaseChanged;

    internal Database() { }

    public int Count()
    {
        lock (databaseLock)
        {
            return database.Count;
        }
    }
    public void AddBook(IBook book)
    {
        lock (databaseLock)
        {
            if (!database.TryGetValue(book.Id, out var existingBook))
            {
                database.Add(book.Id, book);
                databaseChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public void AddBooks(List<IBook> books)
    {
        lock (databaseLock)
        {
            foreach (var book in books)
            {
                AddBook(book);
            }
        }
    }
    public void GetAllBooks(in List<IBook> books)
    {
        lock (databaseLock)
        {
            books.Clear();
            foreach (var book in database.Values)
            {
                books.Add(book);
            }
        }
    }
    public IBook GetBookByID(Guid id)
    {
        lock (databaseLock)
        {
            if (database.TryGetValue(id, out var book))
            {
                return book;
            }
            else
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }
        }
    }
    public void RemoveBook(IBook book)
    {
        lock (databaseLock)
        {
            if (database.Remove(book.Id))
            {
                databaseChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public void RemoveBooks(List<IBook> books)
    {
        lock (databaseLock)
        {
            foreach (var book in books)
            {
                RemoveBook(book);
            }
        }
    }
    public bool TryMarkAsAvailable(Guid id)
    {
        lock (databaseLock)
        {
            if (database.TryGetValue(id, out var book))
            {
                if (book.IsAvailable) return false; // Book is already available
                book.IsAvailable = true;
                databaseChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool TryMarkAsUnavailable(Guid id)
    {
        lock (databaseLock)
        {
            if (database.TryGetValue(id, out var book))
            {
                if (!book.IsAvailable) return false; // Book is already unavailable
                book.IsAvailable = false;
                databaseChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
