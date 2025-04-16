using DataCommon;

namespace DataTest;

[TestClass]
public sealed class IBookTest
{
    [TestMethod]
    public void IBookTest_ConstructorTest_InitStruct()
    {
        IBookInitData bookInitData = new IBookInitData("a", "b", 0, BookType.SciFi);
        var book = IBook.CreateBook(bookInitData);

        // Assert
        Assert.IsNotNull(book);
        Assert.AreEqual("a", book.Title);
        Assert.AreEqual("b", book.Author);
        Assert.AreEqual(0, book.Year);
        Assert.AreEqual(BookType.SciFi, book.Type);
    }

    [TestMethod]
    public void IBookTest_ConstructorTest_Args()
    {
        Guid id = Guid.NewGuid();
        var book = IBook.CreateBook("a", "b", 1, BookType.SciFi, id, false);

        Assert.IsNotNull(book);
        Assert.AreEqual("a", book.Title);
        Assert.AreEqual("b", book.Author);
        Assert.AreEqual(1, book.Year);
        Assert.AreEqual(BookType.SciFi, book.Type);
        Assert.AreEqual(book.Id, id);
        Assert.AreEqual(book.IsAvailable, false);
    }
}
