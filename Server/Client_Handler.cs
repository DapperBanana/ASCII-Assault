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
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                //Basic authentication handling
                if(message.StartsWith("AUTH"))
                {
                    string[] parts = message.Split(' ');
                    if(parts.Length == 3)
                    {
                        string username = parts[1];
                        string password = parts[2];
                        //Add db check.
                        if(SQL_Handler.AuthenticateUser(username, password))
                        {
                            Console.WriteLine($"User {username} authenticated successfully.");
                            authenticated = true;
                        }
                        else
                        {
                            Console.WriteLine($"Authentication failed for user: {username}");
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading from client: {e.Message}");
                server.RemoveClient(this);
            }
        }
    }
}