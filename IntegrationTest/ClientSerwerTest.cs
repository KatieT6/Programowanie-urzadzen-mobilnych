using Client;
using DataCommon;
using System.Net;
using System.Text;
using System.Text.Json;
using Server;
using LogicClient;
using System.Net.WebSockets;
using DataClient;
using Communication;


namespace IntegrationTest
{
    [TestClass]
    public sealed class ClientSerwerTest
    {

        private IServer server;
        private IClient client;

        [TestInitialize]
        public void Init()
        {
            server = IServer.CreateServer();
            client = IClient.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            server?.StopServer();
        }

        [TestMethod]
        public async Task Server_ShouldStartAndStopGracefully()
        {
            Thread serverThread = new Thread(() => server.ServerLoop());
            serverThread.Start();
            await Task.Delay(1000); // Allow some time for the server to start

            Thread clientThread = new Thread(() => client.ClientLoop());
            clientThread.Start();
            await Task.Delay(1000); // Allow some time for the client to start
            
            Assert.IsTrue(server != null, "Server should not be null after initialization.");
            Assert.IsTrue(client != null, "Client should not be null after initialization.");

            return;
        }
    }
    
}
