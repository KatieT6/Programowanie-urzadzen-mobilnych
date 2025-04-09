using PresentationModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Client;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace PresentationViewModel
{
    public class ViewModel : ViewModelBase
    {
        public ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        private SemaphoreSlim signal_ = new(0);
        WSClient _wsClient;

        public ViewModel()
        {
            Console.WriteLine("SSSSSS\n");
            _modelAPI = ModelAbstractApi.CreateModelAPI();
            _library = _modelAPI.Library;

            _books = new ObservableCollection<ModelBook>();
            _borrowedBooks = new ObservableCollection<ModelBook>();

            _wsClient = new WSClient("ws://localhost:5000/ws", messageQueue, signal_);

            _modelAPI.Library.LoadBooks();
            foreach (ModelBook book in _library.GetBooks())
            {
                _books.Add(book);
            }

            new Thread(() =>
            {
                var task = _wsClient.Start();
            }).Start();

            BorrowClick = new RelayCommand(param => BorrowClickHandler(param as ModelBook));
            ReturnClick = new RelayCommand(param => ReturnClickHandler(param as ModelBook));
        }


        public ICommand BorrowClick { get; set; }
        public ICommand ReturnClick { get; set; }

        private void BorrowClickHandler(ModelBook selectedBook)
        {
            if (selectedBook != null && selectedBook.IsAvailable)
            {
                messageQueue.Enqueue("BorrowBook");
                signal_.Release();
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
                messageQueue.Enqueue("ReturnBook");
                signal_.Release();
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
            messageQueue.Enqueue("RefreshBooks");
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
