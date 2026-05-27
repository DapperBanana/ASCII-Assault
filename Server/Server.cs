using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ASCIIAssault_Server
{
    public class Server
    {
        private TcpListener tcpListener;
        private Thread listenerThread;
        private int port = 8080;

        public void StartServer()
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            listenerThread = new Thread(ListenForClients);
            listenerThread.Start();
            Console.WriteLine("Server started on port " + port);
        }

        private void ListenForClients()
        {
            tcpListener.Start();

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();

                Thread clientThread = new Thread(() =>
                {
                    ClientHandler clientHandler = new ClientHandler(client, this);
                    clientHandler.RunClient();
                });
                clientThread.Start();
            }
        }
    }
}
