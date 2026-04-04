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
                    Console.WriteLine($"Received: {data}");

                    // Basic message handling
                    if (!authenticated)
                    {
                        if (data.StartsWith("AUTH "))
                        {
                            string username = data.Substring(5).Trim();
                            //TODO: Authenticate against DB
                            clientName = username;
                            authenticated = true;
                            SendMessage("Authentication successful!");
                            Console.WriteLine($"Client authenticated as {username}");
                        }
                        else
                        {
                            SendMessage("Authentication required. Send 'AUTH <username>'");
                        }
                    }
                    else
                    {
                        // Handle authenticated user messages (game commands, chat, etc.)
                        Console.WriteLine($"Received from {clientName}: {data}");
                        SendMessage($"Server received: {data}"); // Echo for now
                    }
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

        private void SendMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            clientStream.Write(messageBytes, 0, messageBytes.Length);
        }
    }
}
