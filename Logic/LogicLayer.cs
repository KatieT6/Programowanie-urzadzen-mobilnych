using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    internal class LogicLayer : ILogicLayer
    {
        public ILibraryLogic LibraryLogic { get; private set; }

        private IDataLayer DataLayer { get; }

        public LogicLayer(IDataLayer data = default(IDataLayer))
        {
            DataLayer = data;
            LibraryLogic = new LibraryLogic(DataLayer.Library);
        }
    }
}

