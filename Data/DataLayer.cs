using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal class DataLayer : IDataLayer
    {
        public IDatabase Library { get; set; }

        public static IDataLayer CreateDataLayer(IDatabase library = default) { return new DataLayer(library); }

        internal DataLayer(IDatabase library = default)
        {
            Library = library ?? new Database();
        }
    }
}
