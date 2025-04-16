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

        [TestInitialize]
        public void Init()
        {
            server = IServer.CreateServer();
        }

        [TestCleanup]
        public void Cleanup()
        {
            server?.StopServer();
        }

        [TestMethod]
        public void Server_ShouldStartWithoutExceptions()
        {
            // Act
            Exception? ex = null;
            try
            {
                server.ServerLoop();
            }
            catch (Exception e)
            {
                ex = e;
            }

            // Assert
            Assert.IsNull(ex, $"Server failed to start: {ex?.Message}");
        }
    }
    
}
