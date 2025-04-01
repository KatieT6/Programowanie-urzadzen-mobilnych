using Data;

namespace Logic
{
    internal class LogicLayer : ILogicLayer
    {
        public ILibraryLogic LibraryLogic { get; }

        private IDataLayer Data { get; }
    }
}

