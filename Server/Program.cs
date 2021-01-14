using System;
using CardGameServer.WebsocketInternal;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CardGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebsocketServerWrapper server = new WebsocketServerWrapper();
            Console.ReadKey();
            server.wssv.Stop();
        }
    }
}