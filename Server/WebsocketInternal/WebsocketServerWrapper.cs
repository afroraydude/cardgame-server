using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CardGameServer.WebsocketInternal
{
    public class WebsocketServerWrapper
    {
        static string url = "ws://localhost:5001";
        public HttpServer wssv;
        private Dictionary<string, Game> _games;
        public WebsocketServerWrapper()
        {
            Game debugGame = new Game();
            _games = new Dictionary<string, Game>();
            wssv = new HttpServer(80);
            wssv.Log.Level = LogLevel.Debug;
            wssv.AddWebSocketService<WebsocketLobby> ("/lobby", () => new WebsocketLobby(this, _games));
            wssv.AddWebSocketService<WebsocketGame> ("/DebugGame", () => new WebsocketGame("A", debugGame));
            wssv.Start ();
        }

        public void AddGame(string name)
        {
            wssv.AddWebSocketService<WebsocketGame> ($"/{name}", () => new WebsocketGame(name, _games[name]));
        }
    }
}