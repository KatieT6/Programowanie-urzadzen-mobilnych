using Communication;
using DataServer;
using Server;

namespace LogicServer;

public interface ILogicLayer
{
    public IDataLayer DataLayer { get; }
    public IServer Server { get; }
    protected void OnMessageReceived(object sender, Request msg);
    public void ServerLoop();

    public static ILogicLayer CreateLogicLayer(IPublisher publisher)
    {
        return new LogicLayer(publisher);
    }
}
