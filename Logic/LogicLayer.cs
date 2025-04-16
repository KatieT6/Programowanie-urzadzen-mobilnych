using DataClient;
using LogicClient;

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

