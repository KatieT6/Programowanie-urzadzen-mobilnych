using DataClient;

namespace LogicClient
{
    internal class LogicLayer : ILogicLayer
    {
        public ILibraryLogic LibraryLogic { get; private set; }

        private IDataLayer DataLayer { get; }

        public LogicLayer(IDataLayer data = default)
        {
            DataLayer = data;
            LibraryLogic = new LibraryLogic(DataLayer.Library);
        }
    }
}

