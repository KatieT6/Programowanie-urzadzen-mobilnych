using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    interface IDataLayer
    {
        ILibrary Library { get; set; }
        public static IDataLayer CreateDataLayer(ILibrary library = default) { return new DataLayer(library); }
    }
}
