using DataClient;
using LogicClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Client;

namespace LogicClient
{
    public interface ILogicLayer
    {
        public ILibraryLogic LibraryLogic { get; }

        public IClient Client { get; } 

        public void ClientLoop();

        public static ILogicLayer CreateLogicLayer(IDataLayer? data = null)
        {
            return new LogicLayer(data ?? IDataLayer.CreateDataLayer());
        }
    }
}
