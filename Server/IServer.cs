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
    public static IServer CreateServer()
    {
        return new Server();
    }

    public void Init();
    public void ServerLoop();

    public void LendBook(Guid id);
    public void ReturnBook(Guid id);
}
