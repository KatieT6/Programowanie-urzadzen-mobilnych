using Communication;
using DataServer;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogicServer
{
    internal class LogicLayer : ILogicLayer
    {
        IDataLayer dataLayer;
        IServer server;

        public IDataLayer DataLayer => dataLayer;

        public IServer Server => server;

        private HashSet<Guid> subscribers = new();
        private object subscribersLock = new object();

        LogicLayer()
        {
            dataLayer = IDataLayer.CreateDataLayer();
            server = IServer.CreateServer();
            server.messageRecieved += OnMessageReceived!;
        }

        private void HandleNewClientRequest(string argsJson)
        {
            var args = JsonSerializer.Deserialize<NewClientRequest>(argsJson);
            if (args == null) return;
            var clientId = args.Id;
        }

        private void HandleDelClientRequest(string argsJson)
        {
            var args = JsonSerializer.Deserialize<NewClientRequest>(argsJson);
            if (args == null) return;
            var clientId = args.Id;
        }

        private void HandleBorrowBookRequest(string argsJson)
        {
            var args = JsonSerializer.Deserialize<ReturnBorrowRequest>(argsJson);
            if (args == null) return;
            var clientId = args.ClientId;
            var bookId = args.BookId;
            bool result = dataLayer.Database.TryMarkAsUnavailable(bookId);
            
        }

        private void HandleReturnBookRequest(string argsJson)
        {
            var args = JsonSerializer.Deserialize<ReturnBorrowRequest>(argsJson);
            if (args == null) return;
            var clientId = args.ClientId;
            var bookId = args.BookId;
            bool result = dataLayer.Database.TryMarkAsAvailable(bookId);
        }

        private void HandleSubscribeRequest(string argsJson)
        {
            var args = JsonSerializer.Deserialize<SubRequest>(argsJson);
            if (args == null) return;
            var clientId = args.Id;

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
            switch (msg.Name)
            {
                case "NewClientRequest":
                    HandleNewClientRequest(msg.ArgsJson);
                    break;
                case "GetDataRequest":
                    HandleDelClientRequest(msg.ArgsJson);
                    break;
                case "BorrowBook":
                    HandleBorrowBookRequest(msg.ArgsJson);
                    // Handle borrow book request
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
            server.ServerLoop();
        }
    }
}
