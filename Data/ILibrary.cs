using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal interface ILibrary
    {
        public List<IBook> Shelf { get;}
        public void AddBook(IBook book);
        public void RemoveBook(IBook book);
        public void AddBooks(List<IBook> books);
        public void RemoveBooks(List<IBook> books);
        public List<IBook> GetBooksByType(BookType type);
        public List<IBook> GetBooksByID(List<Guid> ids);
    }
}
