using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIAssault_Server
{
    public class ClientHandler
    {
        private TcpClient tcpClient;
        private Server server;
        private NetworkStream clientStream;
        private string? clientName;
        private bool authenticated = false;

        public ClientHandler(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.server = server;
            clientStream = tcpClient.GetStream();
        }

        public void HandleClient()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ClientHandler: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
        }

        private void ProcessMessage(string message)
        {
            Console.WriteLine($"Received from {clientName ?? "Unknown"}: {message}");

            //Basic command processing
            if (message.StartsWith("/auth"))
            {
                Authenticate(message.Substring(6).Trim());
            }
            else
            {
                Broadcast(message);
            }
        }

        private void Authenticate(string credentials)
        {
            //TODO: Implement actual authentication against the DB.
            authenticated = true;
            clientName = credentials;
            Console.WriteLine($"Client authenticated as {clientName}");
        }

        private void Broadcast(string message)
        {
            server.Broadcast(message, this);
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            clientStream.Write(buffer, 0, buffer.Length);
        }

        private void CloseConnection()
        {
            lock (server.clientsLock)
            {
                server.clients.Remove(this);
            }
            tcpClient.Close();
            Console.WriteLine($"Client {clientName ?? "Unknown"} disconnected.");
        }

    }
}
