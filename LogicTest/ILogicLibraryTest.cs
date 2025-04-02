using Data;
using Logic;

namespace LogicTest;

[TestClass]
public sealed class ILogicLibraryTest
{
    [TestMethod]
    public void ILogicLibraryTest_AddBook()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);

        var addedBook = library.GetBookByID(book.Id);

        Assert.IsNotNull(addedBook);
        Assert.AreEqual(addedBook, book);   
    }

    

    [TestMethod]
    public void ILogicLibraryTest_GetBookByID_Correct()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;
        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);

        var result = library.GetBookByID(book.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(book.Id, result.Id);
        Assert.AreEqual(book.Title, result.Title);
        Assert.AreEqual(book.Author, result.Author);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ILogicLibraryTest_GetBookByID_ThrowingException()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;
        var nonExistentBookId = Guid.NewGuid();

        library.GetBookByID(nonExistentBookId);
    }

    [TestMethod]
    public void ILogicLibraryTest_GetBooksByID()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;
        var book1 = IBook.CreateBook("Test Book 1", "Test Author 1", 0, BookType.SciFi);
        var book2 = IBook.CreateBook("Test Book 2", "Test Author 2", 0, BookType.Fantasy);
        library.AddBook(book1);
        library.AddBook(book2);

        var ids = new List<Guid> { book1.Id, book2.Id };

        var result = library.GetBooksByID(ids);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        CollectionAssert.Contains(result, book1);
        CollectionAssert.Contains(result, book2);
    }

   

    [TestMethod]
    public void ILogicLibraryTest_LendBook_Correct()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);

        library.LendBook(book);

        var lentBook = library.GetBookByID(book.Id);
        Assert.IsNotNull(lentBook);
        Assert.IsFalse(lentBook.IsAvailable);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ILogicLibraryTest_LendBook_ThrowingException()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);
        library.LendBook(book);

        library.LendBook(book);
    }

    [TestMethod]
    public void ILogicLibraryTest_LendBookByID_Correct()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);

        library.LendBookByID(book.Id);

        var lentBook = library.GetBookByID(book.Id);
        Assert.IsNotNull(lentBook);
        Assert.IsFalse(lentBook.IsAvailable);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ILogicLibraryTest_LendBookByID_ThrowingException()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);
        library.LendBookByID(book.Id);

        library.LendBookByID(book.Id);
    }

    [TestMethod]
    public void ILogicLibraryTest_ReturnBook_Correct()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);
        library.LendBook(book);

        library.ReturnBook(book);

        var returnedBook = library.GetBookByID(book.Id);
        Assert.IsNotNull(returnedBook);
        Assert.IsTrue(returnedBook.IsAvailable);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ILogicLibraryTest_ReturnBook_ThrowingException()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);

        library.ReturnBook(book);
    }

    [TestMethod]
    public void ILogicLibraryTest_ReturnBooksByID_Correct()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var book = IBook.CreateBook("Test Book", "Test Author", 0, BookType.SciFi);
        library.AddBook(book);
        library.LendBook(book);

        library.ReturnBookByID(book.Id);

        var returnedBook = library.GetBookByID(book.Id);
        Assert.IsNotNull(returnedBook);
        Assert.IsTrue(returnedBook.IsAvailable);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ILogicLibraryTest_ReturnBookByID_ThrowingException()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
        var library = layer.LibraryLogic;

        var nonExistentBookId = Guid.NewGuid();

        library.ReturnBookByID(nonExistentBookId);
    }
}
