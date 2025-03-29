using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal class DataLayer : IDataLayer
    {
        public ILibrary Library { get; set; }

        public static IDataLayer CreateDataLayer(ILibrary library = default) { return new DataLayer(library); }

        internal DataLayer(ILibrary library = default)
        {
            Library = library ?? new Library();
        }
    }
}
