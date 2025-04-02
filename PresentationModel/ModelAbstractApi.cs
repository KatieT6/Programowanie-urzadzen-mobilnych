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
        public IModelLibrary Library { get;}

        public ModelAbstractApi CreateModelAPI(ILogicLayer logic = default)
        {
            return new ModelAPI(logic ?? ILogicLayer.CreateLogicLayer());
        }
    }

    public class ModelAPI : ModelAbstractApi
    {
        private ILogicLayer _logicLayer { get; }
        public ModelAPI(ILogicLayer logic = default)
        {
            _logicLayer = logic;
        }
    }
}
