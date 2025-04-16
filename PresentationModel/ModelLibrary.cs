using Data;
using LogicClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationModel
{
    public class ModelLibrary : INotifyPropertyChanged
    {
        private ILibraryLogic _libraryLogic;
        public ObservableCollection<ModelBook> Books { get; set; }
        public ModelLibrary(ILibraryLogic library)
        {
            _libraryLogic = library;
            Books = new ObservableCollection<ModelBook>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void DeepAdd(ModelBook book)
        {
            var enumType = (BookType)book.Type;
            _libraryLogic.AddBook(IBook.CreateBook(book.Title, book.Author, book.Year, enumType, book.Id, book.IsAvailable));
        }

        public void DeepClear()
        {
            _libraryLogic.Clear();
        }

        public void AddBook(ModelBook book)
        {
            Books.Add(book);
        }
        public List<ModelBook> GetBooks()
        {
            LoadBooks();
            return Books.ToList();
        }

        public List<ModelBook> GetBooksByType(BookType type)
        {
            var books = _libraryLogic.GetBooksByType(type);
            var modelBooks = new List<ModelBook>();
            foreach (var book in books)
            {
                modelBooks.Add(new ModelBook(
                    book.Title,
                    book.Author,
                    book.Year,
                    book.Type.ToString(),
                    book.Id,
                    book.IsAvailable)
                    );
            }
            return modelBooks;
        }

        public ModelBook? GetBookByID(Guid id)
        {
            return Books.FirstOrDefault(b => b.Id == id);
        }

        public void LendBook(ModelBook book)
        {
            Guid id = book.Id;
            _libraryLogic.LendBook(_libraryLogic.GetBookByID(id));
            foreach (ModelBook b in Books)
            {
                if (b.IsAvailable)
                {
                    book.IsAvailable = false;
                }
            }
            OnPropertyChanged(nameof(Books));
        }

        public void LoadBooks()
        {
            var books = _libraryLogic.GetAllBooks();
            Books.Clear();
            foreach (var book in books)
            {
                Books.Add(new ModelBook(
                    book.Title,
                    book.Author,
                    book.Year,
                    book.Type.ToString(),
                    book.Id,
                    book.IsAvailable)
                    );
            }

            OnPropertyChanged(nameof(Books));
        }

        public void RemoveBook(ModelBook book)
        {
            Books.Remove(book);
        }

        public void ReturnBook(ModelBook book)
        {
            Guid id = book.Id;
            _libraryLogic.ReturnBook(_libraryLogic.GetBookByID(id));
            foreach (ModelBook b in Books)
            {
                if (!b.IsAvailable)
                {
                    book.IsAvailable = false;
                }
            }

            OnPropertyChanged(nameof(Books));  
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
