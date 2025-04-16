using Client;
using DataClient;

namespace LogicClient
{
    internal class LogicLayer : ILogicLayer
    {
        public ILibraryLogic LibraryLogic { get; private set; }

        private IDataLayer DataLayer { get; }

        public IClient Client { get; private set; }

        public LogicLayer(IDataLayer data = default)
        {
            DataLayer = data;
            LibraryLogic = new LibraryLogic(DataLayer.Library);
            Client = IClient.CreateClient();
        }

        public void ClientLoop()
        {
            throw new NotImplementedException();
        }
    }
}

