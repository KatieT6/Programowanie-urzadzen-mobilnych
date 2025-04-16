using DataCommon;
using DataServer;


namespace DataTest;

[TestClass]
public sealed class IDatabaseTest
{
    [TestMethod]
    public void IDatabaseTest_AddBook()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Database);
        var database = layer.Database;
        Assert.AreEqual(0, database.Count());
        
        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        var book = IBook.CreateBook(bookInitData);
        
        database.AddBook(book);
        Assert.AreEqual(1, database.Count());
        Assert.AreEqual(book, database.GetBookByID(book.Id));
    }

    [TestMethod]
    public void IDatabaseTest_GetBookByID()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Database);
        var database = layer.Database;
        Assert.AreEqual(0, database.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        var book = IBook.CreateBook(bookInitData);

        database.AddBook(book);
        Assert.AreEqual(1, database.Count());
        Assert.AreEqual(book, database.GetBookByID(book.Id));
    }

    [TestMethod]
    public void IDatabaseTest_RemoveBook()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Database);
        var database = layer.Database;
        Assert.AreEqual(0, database.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        var book = IBook.CreateBook(bookInitData);

        database.AddBook(book);
        Assert.AreEqual(1, database.Count());
        Assert.AreEqual(book, database.GetBookByID(book.Id));

        database.RemoveBook(book);
        Assert.AreEqual(0, database.Count());
    }

    [TestMethod]
    public void IDatabaseTest_AddBooks()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Database);
        var database = layer.Database;
        Assert.AreEqual(0, database.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        IBook book1 = IBook.CreateBook(bookInitData);
        IBook book2 = IBook.CreateBook(bookInitData);
        IBook book3 = IBook.CreateBook(bookInitData);
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        database.AddBooks(books);

        Assert.AreEqual(3, database.Count());
        Assert.IsTrue(database.GetBookByID(book1.Id) == book1);
        Assert.IsTrue(database.GetBookByID(book2.Id) == book2);
        Assert.IsTrue(database.GetBookByID(book3.Id) == book3);
    }

    [TestMethod]
    public void IDatabaseTest_RemoveBooks()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Database);
        var database = layer.Database;
        Assert.AreEqual(0, database.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        IBook book1 = IBook.CreateBook(bookInitData);
        IBook book2 = IBook.CreateBook(bookInitData);
        IBook book3 = IBook.CreateBook(bookInitData);
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        database.AddBooks(books);

        Assert.AreEqual(3, database.Count());
        Assert.IsTrue(database.GetBookByID(book1.Id) == book1);
        Assert.IsTrue(database.GetBookByID(book2.Id) == book2);
        Assert.IsTrue(database.GetBookByID(book3.Id) == book3);

        database.RemoveBooks(books);

        Assert.AreEqual(database.Count(), 0);
    }

    [TestMethod]
    public void IDatabaseTest_TryMarkBookAsAvailableUnavailable()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Database);
        var database = layer.Database;
        Assert.AreEqual(0, database.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        IBook book1 = IBook.CreateBook(bookInitData);
        IBook book2 = IBook.CreateBook(bookInitData);
        IBook book3 = IBook.CreateBook(bookInitData);
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        database.AddBooks(books);

        Assert.IsTrue (database.TryMarkAsUnavailable(book1.Id));
        Assert.IsFalse(database.TryMarkAsUnavailable(book1.Id));
        Assert.IsTrue (database.TryMarkAsAvailable  (book1.Id));
        Assert.IsFalse(database.TryMarkAsAvailable  (book1.Id));
    }
}
