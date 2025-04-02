using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationModel
{
    internal class ModelBook : IModelBook
    {
        private IBook _book;
        private string _title;
        private string _author;
        private int _year;
        private string _bookType;
        private Guid _id;
        private bool _isAvailable;

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

