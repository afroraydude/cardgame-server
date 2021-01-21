using System;
using System.Collections.Generic;
using System.Linq;
using CardGameShared.Data;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace CardGameServer.WebsocketInternal
{
    public class WebsocketLobby : WebSocketBehavior
    {
        private Dictionary<string, Game> _games;
        private WebsocketServerWrapper _server;

        protected override void OnOpen()
        {
            base.OnOpen();
            string id = CreateGame();
            ProperMessage message = new ProperMessage {messageType = MessageType.CreateAccept, messageData = id};
            Send(JsonConvert.SerializeObject(message));
        }

        public WebsocketLobby(WebsocketServerWrapper wrapper, Dictionary<string, Game> games)
        {
            _server = wrapper;
            _games = games;
        }
        
        static string RandomString (int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public string CreateGame()
        {
            string id = RandomString(6);
            Game game = new Game();
            _games[id] = game;
            _server.AddGame(id);
            return id;
        }
    }
}