using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ASCIIAssault_Server
{
    class Game_Master
    {
        static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            SQL_Handler.SetConfiguration(config);

            Server server = new Server();
            server.StartServer();
        }
    }
}
