using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal interface IBook
    {
        string Title { get; set; }
        string Author { get; set; }
        int Year { get; set; }
        BookType Type { get; set; }
        Guid Id { get; set; }
        bool IsAvailable { get; set; }
    }
}
