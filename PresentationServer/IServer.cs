using Communication;

namespace Server;

public interface IServer 
{
    event EventHandler<Request> messageRecieved;
    public void ServerLoop();
    Task SendMessage(Guid clientId, Request request);

    Task BroadcastMessage(Request request);

    public static IServer CreateServer()
    {
        return new Server();
    }

    public void StopServer();
}
