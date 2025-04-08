
using System.Net;

namespace Client;

public class Program
{
    public static void Main(string[] args)
    {
        var client = new WSClient();
        var uri = "ws://localhost:5000/ws";
        var task = client.Start(uri);
        task.Wait();
        Console.WriteLine("AAA");
    }
}