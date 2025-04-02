using PresentationModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PresentationViewModel
{
    public class ViewModel : ViewModelBase
    {
        
    public ViewModel()
        {
            _modelAPI = ModelAbstractApi.CreateModelAPI();
            _library = _modelAPI.Library;

            _books = _library.Books;
            _borrowedBooks = new ObservableCollection<ModelBook>();
            _modelAPI.Library.LoadBooks();
            foreach (ModelBook book in _modelAPI.Library.GetBooks())
            {
                _books.Add(book);
            }

            BorrowClick = new RelayCommand(param => BorrowClickHandler(param as ModelBook));
            ReturnClick = new RelayCommand(param => ReturnClickHandler(param as ModelBook));

        }


        public ICommand BorrowClick { get; set; }
        public ICommand ReturnClick { get; set; }

        private void BorrowClickHandler(ModelBook selectedBook)
        {
            if (selectedBook != null && selectedBook.IsAvailable)
            {
                //_modelAPI.Library.LendBook(selectedBook);
                selectedBook.IsAvailable = false;
                //_books.Remove(selectedBook);
                _borrowedBooks.Add(selectedBook);
                OnPropertyChanged(nameof(Books));
                OnPropertyChanged(nameof(BorrowedBooks));
            }
        }

        private void ReturnClickHandler(ModelBook selectedBook)
        {
            if (selectedBook != null && !selectedBook.IsAvailable)
            {
                //_modelAPI.Library.LendBook(selectedBook);
                selectedBook.IsAvailable = true;
                //_books.Remove(selectedBook);
                _borrowedBooks.Remove(selectedBook);
                OnPropertyChanged(nameof(Books));
                OnPropertyChanged(nameof(BorrowedBooks));
            }
        }


        #region private
        private readonly ModelAbstractApi _modelAPI;
        private ModelLibrary _library;
        private ObservableCollection<ModelBook> _books;
        private ObservableCollection<ModelBook> _borrowedBooks;

        private Timer _timer;

        #endregion

        #region gettr/settr
        public ModelAbstractApi ModelAPI
        {
            get { return _modelAPI; }
        }

        public ModelLibrary Library
        {
            get { return _library; }
            set
            {
                if (_library != value)
                {
                    _library = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ModelBook> Books
        {
            get { return _books; }
            set
            {
                if (_books != value)
                {
                    _books = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ModelBook> BorrowedBooks
        {
            get { return _borrowedBooks; }
            set
            {
                if (_borrowedBooks != value)
                {
                    _borrowedBooks = value;
                    OnPropertyChanged();
                }
            }
        }

        public Timer Timer
        {
            get { return _timer; }
            set
            {
                if (_timer != value)
                {
                    _timer = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}
