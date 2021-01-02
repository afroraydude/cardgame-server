using System;
using System.Collections.Generic;
using CardGameShared.Data;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CardGameServer.WebsocketInternal
{
    public class WebsocketGame : WebSocketBehavior
    {
        
        private string _code;
        private Game _game;
        public WebsocketGame() : this("DebugGame", new Game()) {  }
        public WebsocketGame(string code, Game game)
        {
            _code = code;
            _game = game;
            
        }
        
        protected override void OnMessage(MessageEventArgs e)
        {
            ProperMessage recmsg = JsonConvert.DeserializeObject<ProperMessage>(e.Data);

            if (recmsg.messageType == MessageType.Join)
            {
                Console.WriteLine("Player joined");
                Console.WriteLine($"Data: {recmsg.messageData}");
                List <Player> _players = _game.getPlayers();
                if (_players.Count < 2)
                {
                    Player player = JsonConvert.DeserializeObject<Player>(recmsg.messageData);
                    player.sessionId = ID;
                    _players.Add(player);
                    Console.WriteLine(_players.Count);
                    ProperMessage response = new ProperMessage
                        {messageType = MessageType.JoinAccept, messageData = null};
                    Send(JsonConvert.SerializeObject(response));
                    response = new ProperMessage { messageType = MessageType.OpponentInfo, messageData = JsonConvert.SerializeObject(_players[0])};
                    Send(JsonConvert.SerializeObject(response));
                    if (_players.Count == 2)
                    {
                        response = new ProperMessage { messageType = MessageType.OpponentInfo, messageData = JsonConvert.SerializeObject(player)};
                        Sessions.SendTo(JsonConvert.SerializeObject(response), _players[0].sessionId);
                    }
                }
                else
                {
                    ProperMessage response = new ProperMessage
                        {messageType = MessageType.JoinDeny, messageData = null};
                    Send(JsonConvert.SerializeObject(response));
                }
            }
        }
    }
}