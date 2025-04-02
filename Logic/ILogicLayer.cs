using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface ILogicLayer
    {
        public ILibraryLogic LibraryLogic { get;}

        public static ILogicLayer CreateLogicLayer(IDataLayer? data = null) 
        {
            return new LogicLayer(data ?? IDataLayer.CreateDataLayer()); 
        }
    }
}
