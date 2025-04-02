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
    internal class ModelLibrary : INotifyPropertyChanged
    {
        private ILibraryLogic _libraryLogic;
        public ObservableCollection<ModelBook> Books { get; set; }


        public ModelLibrary(ILibraryLogic library, ObservableCollection<ModelBook> books)
        {
            _libraryLogic = library;
            Books = books;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddBook(ModelBook book)
        {
            Books.Add(book);
        }

        /*public void LendBook(ModelBook book)
        {
            List<ModelBook> borrowedBooks = new List<ModelBook>();

            foreach (ModelBook b in Books)
            {
                if (!b.IsAvailable)
                {
                    borrowedBooks.Add(_libraryLogic.);
                }
            }
        }*/

       /* public void LoadBooks()
        {
            var books = _libraryLogic.GetAllBooks();
            Books.Clear();
            foreach (var book in books)
            {
                Books.Add(new ModelBook
                {
                    Title = book.Title,
                    Author = book.Author,
                    Year = book.Year,
                    BookType = book.Type.ToString(),
                    Id = book.Id,
                    IsAvailable = book.IsAvailable
                });
            }
            OnPropertyChanged(nameof(Books));
        }*/

        public void RemoveBook(ModelBook book)
        {
            Books.Remove(book);
        }

        /*public void ReturnBook(ModelBook book)
        {
            if (!book.IsAvailable)
            {

                _libraryLogic.ReturnBook(book);
                book.IsAvailable = true;
                OnPropertyChanged(nameof(Books));
            }
        }*/

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
