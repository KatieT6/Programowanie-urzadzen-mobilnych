using Communication;
using DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client 
{
    public interface IClient
    {
        Guid ClientId { get; }

        event EventHandler<Request> messageRecieved;

        public static IClient CreateClient()
        {
            return new WSClient();
        }

        public void ClientLoop();
    }
}
