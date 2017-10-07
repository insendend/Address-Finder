using System;
using Classes.Http;
using Classes.Server;
using Classes.Server.Settings;
using Serilog;
using Classes.Parser;

namespace TcpServer
{
    internal class Server
    {
        private static void Main(string[] args)
        {
            Console.Title = "TCP Server";

            #region Server param

            var settings = new AddressServerSettings {Ip = "127.0.0.1", Port = 3333};

            ILogger logger = new LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger();

            IHttpProvider provider = new HttpProvider();
            var parser = new AddressParser(provider);

            #endregion

            var server = new AddressServer(settings, parser, logger);
            server.Start();

            logger.Information("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
