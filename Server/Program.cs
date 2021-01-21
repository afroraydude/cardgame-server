using System;
using System.Threading.Tasks;
using CardGameServer.WebsocketInternal;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CardGameServer
{
    class Program
    {
        private async Task MainAsync(string[] args)
        {
            WebsocketServerWrapper server = new WebsocketServerWrapper();
            await Task.Delay(-1);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up HTTP server");
            Program self = new Program();
            self.MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}