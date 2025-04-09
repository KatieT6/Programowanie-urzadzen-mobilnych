using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationModel
{
    public class ModelBook : INotifyPropertyChanged
    {

        private string _title;
        private string _author;
        private int _year;
        private string _bookType;
        private Guid _id;
        private bool _isAvailable;
        private int _type;
        public ModelBook(string title, string author, int year, string bookType, Guid id, bool isAvaliable)
        {
            _title = title;
            _author = author;
            _year = year;
            _bookType = bookType;
            _id = id;
            _isAvailable = isAvaliable;
        }

        public ModelBook(string title, string author, int year, int type, Guid id, bool isAvaliable)
        {
            _title = title;
            _author = author;
            _year = year;
            _type = type;
            _id = id;
            _isAvailable = isAvaliable;
        }

        public ModelBook(string title, string author, int year, string bookType)
        {
            _title = title;
            _author = author;
            _year = year;
            _bookType = bookType;
            _id = new Guid();
            _isAvailable = true;
        }

        public ModelBook() 
        {
            _title = string.Empty;
            _author = string.Empty;
            _year = 0;
            _bookType = string.Empty;
            _type = 0;
            _id = new Guid();
            _isAvailable = true;
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Author
        {
            get => _author;
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged(nameof(Author));
                }
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                if (_year != value)
                {
                    _year = value;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }

        public string BookType
        {
            get => _bookType;
            set
            {
                if (_bookType != value)
                {
                    _bookType = value;
                    OnPropertyChanged(nameof(BookType));
                }
            }
        }

        public int Type { get => _type; set => _type = value; }

        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    OnPropertyChanged(nameof(IsAvailable));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

