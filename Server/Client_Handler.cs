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
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from {clientName ?? "Unknown"}: {data}");
                    // Process data (authentication, game commands, etc.)

                    // For now, just echo the data back to the client
                    byte[] responseBytes = Encoding.UTF8.GetBytes("Server received: " + data);
                    clientStream.Write(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error handling client {clientName ?? "Unknown"}: {e.Message}");
            }
            finally
            {
                lock (server.clientLock)
                {
                    server.clients.Remove(this);
                }
                tcpClient.Close();
                Console.WriteLine($"Client {clientName ?? "Unknown"} disconnected.");
            }
        }
    }
}
