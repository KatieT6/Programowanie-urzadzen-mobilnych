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
            _modelAPI.Library.LoadBooks();
            foreach (ModelBook book in _modelAPI.Library.GetBooks())
            {
                _books.Add(book);
            }

            BorrowClick = new RelayCommand(param => BorrowClickHandler(param as ModelBook));

        }


        public ICommand BorrowClick { get; set; }

        private void BorrowClickHandler(ModelBook selectedBook)
        {
            if (selectedBook != null)
            {
                _modelAPI.Library.LendBook(selectedBook);
                OnPropertyChanged(nameof(Books));
            }
        }


        #region private
        private readonly ModelAbstractApi _modelAPI;
        private ModelLibrary _library;
        private ObservableCollection<ModelBook> _books;
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
