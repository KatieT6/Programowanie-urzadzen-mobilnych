using Communication;
using DataCommon;
using DataServer;
using Server;
using System.Text.Json;
using System.Xml.Serialization;

namespace LogicServer
{
    internal class LogicLayer : ILogicLayer
    {
        private HashSet<Guid> subscribers = new();
        private object subscribersLock = new object();
        private IDataLayer dataLayer;
        private IServer server;
        private IPublisher publisher;

        public IDataLayer DataLayer => dataLayer;
        public IServer Server => server;

        public LogicLayer(IPublisher publisher)
        {
            dataLayer = IDataLayer.CreateDataLayer();
            server = IServer.CreateServer();
            this.publisher = publisher;
            server.messageRecieved += OnMessageReceived!;
        }

        private void BroadcastLoad()
        {
            List<IBook> books = new List<IBook>();
            dataLayer.GetAllBooks(in books);
            var loadRequest = new LoadRequest(books);

            string xmlMessage = "";
            var xmlSerializer = new XmlSerializer(typeof(LoadRequest));
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, loadRequest);
                xmlMessage = stringWriter.ToString();
            }

            var request = new Request("LoadRequest", xmlMessage);

            BroadcastMessage(request);
        }

        private void SendLoadRequest(Guid clientID)
        {
            List<IBook> books = new List<IBook>();
            dataLayer.GetAllBooks(in books);
            var loadRequest = new LoadRequest(books);
            
            string xmlMessage = "";
            var xmlSerializer = new XmlSerializer(typeof(LoadRequest));
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, loadRequest);
                xmlMessage = stringWriter.ToString();
            }

            var request = new Request("LoadRequest", xmlMessage);
            server.SendMessage(clientID, request);
        }

        private void BroadcastMessage(Request request)
        {
            server.BroadcastMessage(request);
        }

        private void SendBorrowResponse(Guid cliendID, Guid bookId, int result)
        {
            var ackRequest = new ReturnBorrowResponseRequest(result, bookId);

            string xmlMessage = "";
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowResponseRequest));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, ackRequest);
                    xmlMessage = stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating XML serializer: {ex.Message}");
                return;
            }
            

            var request = new Request("BorrowResponse", xmlMessage);
            server.SendMessage(cliendID, request);
        }
        private void SendReturnResponse(Guid cliendID, Guid bookId, int result)
        {
            var ackRequest = new ReturnBorrowResponseRequest(result, bookId);

            string xmlMessage = "";
            var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowResponseRequest));
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, ackRequest);
                xmlMessage = stringWriter.ToString();
            }

            var request = new Request("ReturnResponse", xmlMessage);
            server.SendMessage(cliendID, request);
        }

        private void HandleNewClientRequest(string argsXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(NewClientRequest));
            NewClientRequest request;
            using (var stringReader = new StringReader(argsXml))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not NewClientRequest deserializedRequest) return;
                request = (NewClientRequest)deserialized;
            }

            var clientId = request.Id;

            SendLoadRequest(clientId);
        }
        private void HandleDelClientRequest(string argsXml)
        {
            //Console.Writeline(argsJson);

            var xmlSerializer = new XmlSerializer(typeof(DelClientRequest));
            DelClientRequest request;
            using (var stringReader = new StringReader(argsXml))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not DelClientRequest deserializedRequest) return;
                request = (DelClientRequest)deserialized;
            }

            var clientId = request.Id;
        }

        private void HandleBorrowBookRequest(string argsXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowRequest));
            ReturnBorrowRequest request;
            using (var stringReader = new StringReader(argsXml))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not ReturnBorrowRequest deserializedRequest) return;
                request = (ReturnBorrowRequest)deserialized;
            }
           
            var clientId = request.ClientId;
            var bookId = request.BookId;
            bool result = dataLayer.TryMarkAsUnavailable(bookId);

            if (result)
            {
                SendBorrowResponse(clientId, bookId, 1);
                BroadcastLoad();
            }
            else
            {
                SendBorrowResponse(clientId, bookId, 0);
            }
        }
        private void HandleReturnBookRequest(string argsXml)
        {
            //Console.Writeline(argsJson);

            var xmlSerializer = new XmlSerializer(typeof(ReturnBorrowRequest));
            ReturnBorrowRequest request;
            using (var stringReader = new StringReader(argsXml))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not ReturnBorrowRequest deserializedRequest) return;
                request = (ReturnBorrowRequest)deserialized;
            }

            var clientId = request.ClientId;
            var bookId = request.BookId;
            bool result = dataLayer.TryMarkAsAvailable(bookId);

            if (result)
            {
                SendReturnResponse(clientId, bookId, 1);
                BroadcastLoad();
            }
            else
            {
                SendReturnResponse(clientId, bookId, 0);
            }
        }

        private void HandleSubscribeRequest(string argsXml)
        {
            //Console.Writeline(argsJson);

            var xmlSerializer = new XmlSerializer(typeof(SubRequest));
            SubRequest request;
            using (var stringReader = new StringReader(argsXml))
            {
                var deserialized = xmlSerializer.Deserialize(stringReader);
                if (deserialized is not SubRequest deserializedRequest) return;
                request = (SubRequest)deserialized;
            }

            
            if (request == null) return;
            var clientId = request.Id;

            lock (subscribersLock)
            {
                if (subscribers.Contains(clientId))
                {
                    subscribers.Remove(clientId);
                }
                else
                {
                    subscribers.Add(clientId);
                }
            }
        }

        public void OnMessageReceived(object sender, Request msg)
        {
            Console.WriteLine($"{msg.Name} {msg.ArgsJson}");
            switch (msg.Name)
            {
                case "NewClient":
                    HandleNewClientRequest(msg.ArgsJson);
                    break;
                case "DelClient":
                    HandleDelClientRequest(msg.ArgsJson);
                    break;
                case "BorrowBook":
                    HandleBorrowBookRequest(msg.ArgsJson);
                    break;
                case "ReturnBook":
                    HandleReturnBookRequest(msg.ArgsJson);
                    break;
                case "Subscribe":
                    HandleSubscribeRequest(msg.ArgsJson);   
                    break;
                default:
                    break;
            }
        }

        public void ServerLoop()
        {
            if (server == null) return;
            var bookInitData = publisher.GetNewBook();
            dataLayer.AddBook(IBook.CreateBook(bookInitData));

            _ = Task.Run(() =>
            {
                while (true)
                {   
                    Thread.Sleep(10000);
                    var bookInitData = publisher.GetNewBook();
                    dataLayer.AddBook(IBook.CreateBook(bookInitData));
                    BroadcastLoad();
                }
            });

            server.ServerLoop();
        }
    }
}
