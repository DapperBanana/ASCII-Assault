using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading; // Import the Threading namespace
using System.Threading.Tasks;

namespace ASCIIAssault_Server
{
    public class Server
    {
        private TcpListener? tcpListener;
        public List<ClientHandler> clients = new List<ClientHandler>();
        public readonly object clientsLock = new object();

        public void StartServer()
        {
            tcpListener = new TcpListener(IPAddress.Any, 6969);
            tcpListener.Start();
            Console.WriteLine("Server started.");

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("New client connected.");

                ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                lock (clientsLock)
                {
                    clients.Add(clientHandler);
                }

                Thread clientThread = new Thread(() => clientHandler.HandleClient());
                clientThread.Start();
            }
        }

        public void Broadcast(string message, ClientHandler sender)
        {
            lock (clientsLock)
            {
                foreach (var client in clients)
                {
                    if (client != sender)
                    {
                        client.SendMessage(message);
                    }
                }
            }
        }

    }
}
