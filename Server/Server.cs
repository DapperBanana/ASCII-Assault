using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace ASCIIAssault_Server
{
    public class Server
    {
        private TcpListener? tcpListener;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private IConfiguration? _config;
        private GameState _gameState = new GameState();

        public void SetConfiguration(IConfiguration config)
        {
            _config = config;
        }

        public void StartServer()
        {
            int port = int.Parse(_config?["Port"] ?? "5000");

            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Console.WriteLine("Server started on port " + port);

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Client connected");

                ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                lock (clients)
                {
                    clients.Add(clientHandler);
                }

                Thread clientThread = new Thread(() => clientHandler.RunClient());
                clientThread.Start();
            }
        }

        public void Broadcast(string message, ClientHandler sender)
        {
            lock (clients)
            {
                foreach (var client in clients)
                {
                    if (client != sender && client.IsAuthenticated())
                    {
                        client.SendMessage(message);
                    }
                }
            }
        }

        public GameState GetGameState()
        {
            GameState currentState = new GameState();
            lock (clients)
            {
                foreach (var client in clients)
                {
                    if (client.IsAuthenticated() && client.ClientName != null)
                    {
                        currentState.PlayerPositions[client.ClientName] = (client.X, client.Y);
                    }
                }
            }
            return currentState;
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (clients)
            {
                clients.Remove(client);
            }
        }
    }
}