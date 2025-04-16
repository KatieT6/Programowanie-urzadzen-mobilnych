using DataCommon;

namespace Communication
{
    public class LoadRequest
    {
        private List<Book> books;

        public List<Book> Books
        {
            get { return books; }
            set { books = value; }
        }

        public LoadRequest()
        {
            books = new List<Book>();
        }

        public LoadRequest(List<Book> books)
        {
            this.books = books;
        }

        public LoadRequest(List<IBook> books)
        {
            this.books = new List<Book>(books.Count);

            foreach (var book in books)
            {
                this.books.Add(new Book(book.Title, book.Author, book.Year, book.Type, book.Id, book.IsAvailable));
            }
        }
    }
}
