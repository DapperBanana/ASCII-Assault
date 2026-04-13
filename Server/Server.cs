using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

            Console.WriteLine("Server started. Listening on port 6969");

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                lock (clientsLock)
                {
                    clients.Add(clientHandler);
                }

                Thread clientThread = new Thread(() => clientHandler.HandleClient());
                clientThread.Start();
            }
        }

        public void BroadcastMessage(string message, ClientHandler sourceClient)
        {
            lock (clientsLock)
            {
                foreach (var client in clients)
                {
                    if (client != sourceClient && client.GetClientName() != null)
                    {
                        client.SendMessage(message);
                    }
                }
            }
        }

        public void RemoveClient(ClientHandler clientToRemove)
        {
            lock (clientsLock)
            {
                clients.Remove(clientToRemove);
            }
            Console.WriteLine("Client disconnected.");
        }

        public bool AuthenticateUser(string username, string password)
        {
            return SQL_Handler.CheckCredentials(username, password);
        }
    }
}
