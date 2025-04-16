namespace Server;

public static class App
{
    public static void Main(string[] args)
    {
            var client = new WSClient();

        IServer server = IServer.CreateServer();
        server.Init();
        server.ServerLoop();
    }
}