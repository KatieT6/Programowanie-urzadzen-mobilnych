using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IBook : INotifyPropertyChanged
    {
        string Title { get; set; }
        string Author { get; set; }
        int Year { get; set; }
        BookType Type { get; set; }
        Guid Id { get; set; }
        bool IsAvailable { get; set; }
    }
}
