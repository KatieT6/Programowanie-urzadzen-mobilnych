using DataClient;
using DataCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicClient
{
    public interface ILibraryLogic : INotifyPropertyChanged
    {
        public IDatabase Library { get; set; }

        public void Clear();
        public void AddBook(IBook book);
        public void LendBook(IBook book);
        public void ReturnBook(IBook book);
        public void LendBookByID(Guid id);
        public void ReturnBookByID(Guid id);
        public List<IBook> GetBooksByType(BookType type);
        public List<IBook> GetBooksByID(List<Guid> ids);

        public IBook GetBookByID(Guid id);

        public List<IBook> GetAllBooks();
    }
}
