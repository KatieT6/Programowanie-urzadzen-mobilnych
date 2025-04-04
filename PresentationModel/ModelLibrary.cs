﻿using Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public void AddBook(ModelBook book)
        {
            Books.Add(book);
        }
        public List<ModelBook> GetBooks()
        {
            LoadBooks();
            return Books.ToList();
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
