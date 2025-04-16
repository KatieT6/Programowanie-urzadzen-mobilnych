using PresentationModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using Communication;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections;
using DataClient;
using DataCommon;

namespace PresentationViewModel
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            _modelAPI = ModelAbstractApi.CreateModelAPI();
            _library = _modelAPI.Library;

            _books = new ObservableCollection<ModelBook>();
            _borrowedBooks = new ObservableCollection<ModelBook>();

            _bookTypes = new ObservableCollection<string> { "All" };
            foreach (var type in Enum.GetValues(typeof(BookType)))
            {
                if (type != null)
                {
                    _bookTypes.Add(type.ToString());
                }
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

                _library.LendBook(selectedBook);
                RefreshLibrary();
                _borrowedBooks.Add(selectedBook);
                OnPropertyChanged(nameof(Books));
                OnPropertyChanged(nameof(BorrowedBooks));
            }
        }

        private void ReturnClickHandler(ModelBook selectedBook)
        {
            if (selectedBook != null && !selectedBook.IsAvailable)
            {
                selectedBook.IsAvailable = true;
                _library.ReturnBook(selectedBook);
                RefreshLibrary();
                _borrowedBooks.Remove(selectedBook);
                OnPropertyChanged(nameof(Books));
                OnPropertyChanged(nameof(BorrowedBooks));
            }
        }

        private void RefreshLibrary()
        {

            _books.Clear();
            foreach (ModelBook book in _library.GetBooks())
            {
                _books.Add(book);
            }
        }


        #region private
        private readonly ModelAbstractApi _modelAPI;
        private ModelLibrary _library;

        private ObservableCollection<ModelBook> _books;
        private ObservableCollection<ModelBook> _borrowedBooks;

        private string _selectedBookType = "All";
        private ObservableCollection<string> _bookTypes;

        private bool booksChanged = false;
        private object _booksChangedLock = new();

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

        public ObservableCollection<string> BookTypes
        {
            get => _bookTypes;
            set
            {
                _bookTypes = value;
                OnPropertyChanged();
            }
        }

        public string SelectedBookType
        {
            get => _selectedBookType;
            set
            {
                if (_selectedBookType != value)
                {
                    _selectedBookType = value;
                    OnPropertyChanged();
                    lock (_booksChangedLock)
                    {
                        booksChanged = true;
                    }
                }
            }
        }
        
        #endregion

    }
}

