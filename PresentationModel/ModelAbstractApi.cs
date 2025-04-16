using LogicClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PresentationModel
{
    public abstract class ModelAbstractApi
    {
        public abstract ModelLibrary Library { get; }

        static public ModelAbstractApi CreateModelAPI(ILogicLayer? logic = null)
        {
            return new ModelAPI(logic ?? ILogicLayer.CreateLogicLayer());
        }
    }

    public class ModelAPI : ModelAbstractApi
    {
        private ILogicLayer _logicLayer { get; }

        public override ModelLibrary Library => new ModelLibrary(_logicLayer.LibraryLogic);


        public ModelAPI(ILogicLayer logic)
        {
            _logicLayer = logic;
            _logicLayer.ClientLoop();
        }
    }
}
