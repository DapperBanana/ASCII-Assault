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

        public void ProcessClient()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from {clientName ?? "Unknown"}: {dataReceived}");
                    server.Broadcast($"{clientName ?? "Unknown"}: {dataReceived}", this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error handling client {clientName ?? "Unknown"}: {e.Message}");
            }
            finally
            {
                server.RemoveClient(this);
                tcpClient.Close();
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                clientStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending message to {clientName ?? "Unknown"}: {e.Message}");
            }
        }

        public void SetClientName(string name)
        {
            clientName = name;
        }

        public string? GetClientName()
        {
            return clientName;
        }

        public bool IsAuthenticated()
        {
            return authenticated;
        }

        public void SetAuthenticated(bool auth)
        {
            authenticated = auth;
        }
    }
}
