namespace Server;

public static class App
{
    public static void Main(string[] args)
    {
        IServer server = IServer.CreateServer();
        server.Init();
        server.ServerLoop();
    }
}