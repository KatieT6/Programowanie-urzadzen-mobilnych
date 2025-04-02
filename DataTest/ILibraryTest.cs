using Data;
using System.Linq.Expressions;

namespace DataTest;

[TestClass]
public sealed class ILibraryTest
{
    [TestMethod]
    public void ILibraryTest_AddBook()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book = IBook.CreateBook();
        library.AddBook(book);
        Assert.AreEqual(1, library.Shelf.Count);
        Assert.AreEqual(book, shelf[0]);
    }

    [TestMethod]
    public void ILibraryTest_GetBookByID()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book = IBook.CreateBook();
        library.AddBook(book);
        Assert.AreEqual(1, library.Shelf.Count);
        
        var book1 = library.GetBookByID(book.Id);
        Assert.AreEqual(book, book1);
    }

    [TestMethod]
    public void ILibraryTest_RemoveBook()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book = IBook.CreateBook();
        library.AddBook(book);
        Assert.AreEqual(1, library.Shelf.Count);

        var book1 = library.GetBookByID(book.Id);
        Assert.AreEqual(book, book1);

        library.RemoveBook(book);
        Assert.AreEqual(0, library.Shelf.Count);
    }

    [TestMethod]
    public void ILibraryTest_AddBooks()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book1 = IBook.CreateBook();
        IBook book2 = IBook.CreateBook();
        IBook book3 = IBook.CreateBook();
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        library.AddBooks(books);

        Assert.AreEqual(3, library.Shelf.Count);
        Assert.IsTrue(library.Shelf.Contains(book1));
        Assert.IsTrue(library.Shelf.Contains(book2));
        Assert.IsTrue(library.Shelf.Contains(book3));
    }

    [TestMethod]
    public void ILibraryTest_GetBooksByID()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book1 = IBook.CreateBook();
        IBook book2 = IBook.CreateBook();
        IBook book3 = IBook.CreateBook();
        List<IBook> books = new List<IBook> { book1, book2, book3 };
        library.AddBooks(books);
        Assert.AreEqual(3, library.Shelf.Count);

        var retrievedBooks = library.GetBooksByID(new List<Guid> { book1.Id, book3.Id });

        Assert.AreEqual(2, retrievedBooks.Count);
        Assert.IsTrue(retrievedBooks.Contains(book1));
        Assert.IsTrue(retrievedBooks.Contains(book3));
        Assert.IsFalse(retrievedBooks.Contains(book2));
    }

    [TestMethod]
    public void ILibraryTest_RemoveBooks()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book1 = IBook.CreateBook();
        IBook book2 = IBook.CreateBook();
        IBook book3 = IBook.CreateBook();
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        library.AddBooks(books);
        Assert.AreEqual(3, library.Shelf.Count);

        library.RemoveBooks(new List<IBook> { book1, book3 });
        Assert.AreEqual(1, library.Shelf.Count);
        Assert.IsFalse(library.Shelf.Contains(book1));
        Assert.IsTrue(library.Shelf.Contains(book2));
        Assert.IsFalse(library.Shelf.Contains(book3));
    }

    [TestMethod]
    public void ILibraryTest_GetBooksByType()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book1 = IBook.CreateBook();
        book1.Type = BookType.NonFiction;
        IBook book2 = IBook.CreateBook();
        book2.Type = BookType.SciFi;
        IBook book3 = IBook.CreateBook();
        book3.Type = BookType.NonFiction;
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        library.AddBooks(books);
        Assert.AreEqual(3, library.Shelf.Count);

        var fictionBooks = library.GetBooksByType(BookType.NonFiction);

        Assert.AreEqual(2, fictionBooks.Count);
        Assert.IsTrue(fictionBooks.Contains(book1));
        Assert.IsTrue(fictionBooks.Contains(book3));
        Assert.IsFalse(fictionBooks.Contains(book2));
    }

    [TestMethod]
    public void ILibraryTest_MarkBookAsAvailable()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book = IBook.CreateBook();
        book.IsAvailable = false;
        library.AddBook(book);
        Assert.AreEqual(1, library.Shelf.Count);
        Assert.IsFalse(book.IsAvailable);

        library.MarkBookAsAvailable(book);
        Assert.IsTrue(book.IsAvailable);
    }

    [TestMethod]
    public void ILibraryTest_MarkBookAsUnavailable()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);
        var shelf = library.Shelf;

        IBook book = IBook.CreateBook();
        book.IsAvailable = true;
        library.AddBook(book);
        Assert.AreEqual(1, library.Shelf.Count);
        Assert.IsTrue(book.IsAvailable);

        library.MarkBookAsUnavailable(book);
        Assert.IsFalse(book.IsAvailable);
    }

    [TestMethod]
    public void ILibraryTest_GetAllBooks()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();
        Assert.IsNotNull(layer.Library);
        var library = layer.Library;
        library.Shelf.Clear();
        Assert.IsNotNull(library.Shelf);
        Assert.AreEqual(0, library.Shelf.Count);

        IBook book1 = IBook.CreateBook();
        IBook book2 = IBook.CreateBook();
        IBook book3 = IBook.CreateBook();
        List<IBook> books = new List<IBook> { book1, book2, book3 };

        library.AddBooks(books);
        Assert.AreEqual(3, library.Shelf.Count);

        var allBooks = library.GetAllBooks();
        Assert.AreEqual(3, allBooks.Count);
        Assert.IsTrue(allBooks.Contains(book1));
        Assert.IsTrue(allBooks.Contains(book2));
        Assert.IsTrue(allBooks.Contains(book3));
    }
}
