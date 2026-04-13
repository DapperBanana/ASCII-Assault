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
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + data);

                    // Process the data (e.g., authentication, game commands)
                    if (!authenticated)
                    {
                        if (data.StartsWith("AUTH:"))
                        {
                            string[] parts = data.Substring(5).Split(':');
                            if (parts.Length == 2)
                            {
                                string username = parts[0];
                                string password = parts[1];
                                if (server.AuthenticateUser(username, password))
                                {
                                    authenticated = true;
                                    clientName = username;
                                    SendMessage("AUTH_OK");
                                    server.BroadcastMessage(clientName + " joined the game!", this);
                                }
                                else
                                {
                                    SendMessage("AUTH_FAIL");
                                }
                            }
                            else
                            {
                                SendMessage("AUTH_INVALID");
                            }
                        }
                        else
                        {
                            SendMessage("NOT_AUTH");
                        }
                    }
                    else
                    {
                        server.BroadcastMessage(clientName + ": " + data, this);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error handling client: " + e.Message);
            }
            finally
            {
                server.RemoveClient(this);
                tcpClient.Close();
            }
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        public string? GetClientName()
        {
            return clientName;
        }

    }
}
