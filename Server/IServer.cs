using Communication;
using Data;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server;

public interface IServer
{
    event EventHandler<Request> messageRecieved;
    public void ServerLoop();
    Task SendMessage(Guid clientId, Request request);
    public static IServer CreateServer()
    {
        return new Server();
    }
}
