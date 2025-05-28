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
        Assert.AreEqual(0, layer.Count());
        
        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        var book = IBook.CreateBook(bookInitData);

        layer.AddBook(book);
        Assert.AreEqual(1, layer.Count());
        Assert.AreEqual(book, layer.GetBookByID(book.Id));
    }

    [TestMethod]
    public void IDatabaseTest_GetBookByID()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.AreEqual(0, layer.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        var book = IBook.CreateBook(bookInitData);

        layer.AddBook(book);
        Assert.AreEqual(1, layer.Count());
        Assert.AreEqual(book, layer.GetBookByID(book.Id));
    }

    [TestMethod]
    public void IDatabaseTest_RemoveBook()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.AreEqual(0, layer.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        var book = IBook.CreateBook(bookInitData);

        layer.AddBook(book);
        Assert.AreEqual(1, layer.Count());
        Assert.AreEqual(book, layer.GetBookByID(book.Id));

        layer.RemoveBook(book);
        Assert.AreEqual(0, layer.Count());
    }

    [TestMethod]
    public void IDatabaseTest_AddBooks()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.AreEqual(0, layer.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        IBook book1 = IBook.CreateBook(bookInitData);
        IBook book2 = IBook.CreateBook(bookInitData);
        IBook book3 = IBook.CreateBook(bookInitData);
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        layer.AddBooks(books);

        Assert.AreEqual(3, layer.Count());
        Assert.IsTrue(layer.GetBookByID(book1.Id) == book1);
        Assert.IsTrue(layer.GetBookByID(book2.Id) == book2);
        Assert.IsTrue(layer.GetBookByID(book3.Id) == book3);
    }

    [TestMethod]
    public void IDatabaseTest_RemoveBooks()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        
        Assert.AreEqual(0, layer.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        IBook book1 = IBook.CreateBook(bookInitData);
        IBook book2 = IBook.CreateBook(bookInitData);
        IBook book3 = IBook.CreateBook(bookInitData);
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        layer.AddBooks(books);

        Assert.AreEqual(3, layer.Count());
        Assert.IsTrue(layer.GetBookByID(book1.Id) == book1);
        Assert.IsTrue(layer.GetBookByID(book2.Id) == book2);
        Assert.IsTrue(layer.GetBookByID(book3.Id) == book3);

        layer.RemoveBooks(books);

        Assert.AreEqual(layer.Count(), 0);
    }

    [TestMethod]
    public void IDatabaseTest_TryMarkBookAsAvailableUnavailable()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();

        Assert.AreEqual(0, layer.Count());

        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        IBook book1 = IBook.CreateBook(bookInitData);
        IBook book2 = IBook.CreateBook(bookInitData);
        IBook book3 = IBook.CreateBook(bookInitData);
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        layer.AddBooks(books);

        Assert.IsTrue (layer.TryMarkAsUnavailable(book1.Id));
        Assert.IsFalse(layer.TryMarkAsUnavailable(book1.Id));
        Assert.IsTrue (layer.TryMarkAsAvailable  (book1.Id));
        Assert.IsFalse(layer.TryMarkAsAvailable  (book1.Id));
    }
}
