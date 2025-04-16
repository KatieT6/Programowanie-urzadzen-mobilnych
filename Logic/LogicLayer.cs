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
            Client = IClient.CreateClient();
            LibraryLogic = new LibraryLogic(DataLayer.Library, Client);
        }

        public void ClientLoop()
        {
            if (Client == null) return;
            Client.ClientLoop();
        }
    }
}

