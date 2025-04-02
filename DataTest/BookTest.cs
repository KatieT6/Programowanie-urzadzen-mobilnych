using Data;
using System.Net.Mail;

namespace Data
{
    [TestClass]
    public class BookTest
    {
        [TestMethod]
        public void ConstructorTest_Default()
        {
            var book = IBook.CreateBook();

            // Assert
            Assert.IsNotNull(book);
            Assert.AreEqual(string.Empty, book.Title);
            Assert.AreEqual(string.Empty, book.Author);
            Assert.AreEqual(0, book.Year);
            Assert.AreEqual(BookType.NonFiction, book.Type);
        }

        [TestMethod]
        public void ConstructorTest_Arg()
        {
            var book = IBook.CreateBook("a", "b", 0, BookType.SciFi);

            // Assert
            Assert.IsNotNull(book);
            Assert.AreEqual("a", book.Title);
            Assert.AreEqual("b", book.Author);
            Assert.AreEqual(0, book.Year);
            Assert.AreEqual(BookType.SciFi, book.Type);
        }

        [TestMethod]
        public void ConstructorTest_InitStruct()
        {
            BookInit bookInit = new BookInit("a", "b", 1, BookType.Romance);
            var book = IBook.CreateBook(bookInit);

            Assert.IsNotNull(book);
            Assert.AreEqual("a", book.Title);
            Assert.AreEqual("b", book.Author);
            Assert.AreEqual(1, book.Year);
            Assert.AreEqual(BookType.Romance, book.Type);
        }
    }
}
