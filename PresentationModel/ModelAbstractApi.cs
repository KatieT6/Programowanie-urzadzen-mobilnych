using Logic;
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
        public ModelAbstractApi CreateModelAPI(ILogicLayer logic = default)
        {
            return new ModelAPI(logic ?? ILogicLayer.CreateLogicLayer());
        }
    }

    public class ModelAPI : ModelAbstractApi
    {
        private ILogicLayer _logicLayer { get; }

        public  ModelLibrary Library;

        public ModelAPI(ILogicLayer logic = default)
        {
            _logicLayer = logic;
            Library = new ModelLibrary(_logicLayer.LibraryLogic);
        }
    }
}
