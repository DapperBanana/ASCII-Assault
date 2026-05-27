using System;
using System.Net.Sockets;
using System.Text;

namespace ASCIIAssault_Client
{
    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;

        public Client(string serverAddress, int serverPort)
        {
            try
            {
                client = new TcpClient(serverAddress, serverPort);
                stream = client.GetStream();
                Console.WriteLine("Connected to server.");

                // Start listening for server messages in a separate thread or Task.
                Task.Run(() => ReceiveData());

                // Start sending data to the server.
                SendData();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to server: {e.Message}");
            }
        }

        private void ReceiveData()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // Server disconnected.
                        Console.WriteLine("Disconnected from server.");
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessData(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error receiving data: {e.Message}");
                    break;
                }
            }

            client.Close();
        }

        private void SendData()
        {
            while (true)
            {
                string? message = Console.ReadLine();
                if (string.IsNullOrEmpty(message))
                    continue;

                byte[] data = Encoding.UTF8.GetBytes(message);
                try
                {
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending data: {e.Message}");
                    break;
                }
            }
        }

        private void ProcessData(string data)
        {
            // Placeholder for game logic
            Console.WriteLine($"Received: {data}");
            //TODO: parse GameState and render to screen
        }

        public static void Main(string[] args)
        {
            string serverAddress = "127.0.0.1"; // Localhost
            int serverPort = 8080;
            Client client = new Client(serverAddress, serverPort);

            // Keep the console open until the client disconnects.
            Console.ReadKey();
        }
    }
}
