using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IDataLayer
    {
        public IDatabase Library { get; set; }
        public static IDataLayer CreateDataLayer(IDatabase library = default) { return new DataLayer(library); }
    }
}
