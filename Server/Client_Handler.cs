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
                    Console.WriteLine($"Received from {clientName ?? "Unauthenticated Client"}: {dataReceived}");

                    // Basic command handling (example: BROADCAST)
                    if (dataReceived.ToUpper().StartsWith("BROADCAST ") && authenticated)
                    {
                        string message = dataReceived.Substring(10);
                        server.Broadcast($"{clientName}: {message}", this);
                    }
                    else if (dataReceived.ToUpper().StartsWith("AUTH "))
                    {
                        string[] parts = dataReceived.Substring(5).Split(' ');
                        if (parts.Length == 2)
                        {
                            string username = parts[0];
                            string password = parts[1];

                            if (SQL_Handler.AuthenticateUser(username, password))
                            {
                                clientName = username;
                                authenticated = true;
                                SendMessage("Authentication successful");
                                Console.WriteLine($"Client {clientName} authenticated.");
                            }
                            else
                            {
                                SendMessage("Authentication failed");
                                Console.WriteLine("Authentication failed for user.");
                            }
                        }
                        else
                        {
                            SendMessage("Invalid authentication format.  Use AUTH <username> <password>");
                        }

                    }
                    else
                    {
                        if (!authenticated)
                        {
                            Console.WriteLine("Unauthenticated client attempted to send a command.");
                            SendMessage("Authentication required. Use AUTH <username> <password>");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error handling client: {e.Message}");
            }
            finally
            {
                server.RemoveClient(this);
                tcpClient.Close();
                Console.WriteLine($"Client {clientName ?? "Unauthenticated Client"} disconnected.");
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                clientStream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending message to client: {e.Message}");
            }
        }

        public string? GetClientName()
        {
            return clientName;
        }
    }
}
