using DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IClient: IObserver<IBook>
    {
        public event MessageReceivedHandler MessageReceived;

        public delegate void MessageReceivedHandler(string message);

    }
}
