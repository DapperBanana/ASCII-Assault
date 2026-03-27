using System;


namespace ASCIIAssault_Server
{
    class Game_Master
    {
        static void Main()
        {
            Server server = new Server();
            server.StartServer();
        }
    }
}