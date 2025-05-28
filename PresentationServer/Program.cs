using DataCommon;
using LogicServer;

public static class Program
{
    public static void Main(string[] args)
    {
        ILogicLayer logicLayer = ILogicLayer.CreateLogicLayer(IPublisher.CreatePublisher());
        logicLayer.ServerLoop();
    }
}