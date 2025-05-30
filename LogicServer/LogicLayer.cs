﻿using Communication;
using DataCommon;
using DataServer;
using Server;
using System.Text.Json;

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
            var request = new Request("LoadRequest", JsonSerializer.Serialize(loadRequest));

            BroadcastMessage(request);
        }

        private void SendLoadRequest(Guid clientID)
        {
            List<IBook> books = new List<IBook>();
            dataLayer.GetAllBooks(in books);
            var loadRequest = new LoadRequest(books);
            var request = new Request("LoadRequest", JsonSerializer.Serialize(loadRequest));
            server.SendMessage(clientID, request);
        }

        private void BroadcastMessage(Request request)
        {
            server.BroadcastMessage(request);
        }

        private void SendBorrowResponse(Guid cliendID, Guid bookId, int result)
        {
            var ackRequest = new ReturnBorrowResponseRequest(result, bookId);
            var request = new Request("BorrowResponse", JsonSerializer.Serialize(ackRequest));
            server.SendMessage(cliendID, request);
        }

        private void SendReturnResponse(Guid cliendID, Guid bookId, int result)
        {
            var ackRequest = new ReturnBorrowResponseRequest(result, bookId);
            var request = new Request("ReturnResponse", JsonSerializer.Serialize(ackRequest));
            server.SendMessage(cliendID, request);
        }

        private void HandleNewClientRequest(string argsJson)
        {
            //Console.Writeline(argsJson);
            var args = JsonSerializer.Deserialize<NewClientRequest>(argsJson);
            if (args == null) return;
            var clientId = args.Id;

            SendLoadRequest(clientId);
        }

        private void HandleDelClientRequest(string argsJson)
        {
            //Console.Writeline(argsJson);
            var args = JsonSerializer.Deserialize<NewClientRequest>(argsJson);
            if (args == null) return;
            var clientId = args.Id;
        }

        private void HandleBorrowBookRequest(string argsJson)
        {
            //Console.Writeline(argsJson);
            var args = JsonSerializer.Deserialize<ReturnBorrowRequest>(argsJson);
            if (args == null) return;
            var clientId = args.ClientId;
            var bookId = args.BookId;
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

        private void HandleReturnBookRequest(string argsJson)
        {
            //Console.Writeline(argsJson);
            var args = JsonSerializer.Deserialize<ReturnBorrowRequest>(argsJson);
            if (args == null) return;
            var clientId = args.ClientId;
            var bookId = args.BookId;
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

        private void HandleSubscribeRequest(string argsJson)
        {
            //Console.Writeline(argsJson);
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
            //Console.Writeline($"{msg.Name} {msg.ArgsJson}");
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
