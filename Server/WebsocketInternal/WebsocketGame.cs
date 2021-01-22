using System;
using System.Collections.Generic;
using System.Net.Sockets;
using CardGameShared.Data;
using CardGameShared.Exception;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CardGameServer.WebsocketInternal
{
    public class WebsocketGame : WebSocketBehavior
    {
        
        private string _code;
        private Game _game;
        public WebsocketGame() : this("000000", new Game()) {  }
        public WebsocketGame(string code, Game game)
        {
            _code = code;
            _game = game;

            if (code == "000000")
            {
                Player decoy = new Player
                {
                    actions = new[]
                    {
                        ActionType.Shield, ActionType.Shield, ActionType.Sword, ActionType.HeavySwordH,
                        ActionType.HeavySwordS
                    },
                    name = "Me",
                    avatar = Avatar.TVGuy,
                    lockedIn = true,
                    sessionId = "testuser1"
                };
                
                _game.AddPlayer(decoy);
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            throw new CardGameServerException(ErrorCode.Unknown);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("User left game: " + _code);
            List<Player> _players = _game.getPlayers();
            Console.WriteLine("Tring to find player with session ID " + ID);
            var thisPlayerIndex = _players.IndexOf(_players.Find(player => player.sessionId == ID));
            if (thisPlayerIndex >= 0)
            {
                Console.WriteLine("User was player, removing.");
                var player = _players[thisPlayerIndex];
                _game.RemovePlayer(player);
                Console.WriteLine("Current players: " + _game.getPlayers().Count);
            }
            else
            {
                Console.WriteLine("User was not player, ignoring.");
            }
            base.OnClose(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            ProperMessage recmsg = JsonConvert.DeserializeObject<ProperMessage>(e.Data);
            Console.WriteLine($"Message received: {e.Data}");

            if (recmsg.messageType == MessageType.Join)
            {
                Console.WriteLine("Player joined");
                Console.WriteLine($"Data: {recmsg.messageData}");
                List <Player> _players = _game.getPlayers();
                if (_players.Count < 2)
                {
                    Player player = JsonConvert.DeserializeObject<Player>(recmsg.messageData);
                    player.sessionId = ID;
                    //_players.Add(player);
                    _game.AddPlayer(player);
                    Console.WriteLine(_game.getPlayers().Count);
                    Console.WriteLine(JsonConvert.SerializeObject(_game.getPlayers()));
                    ProperMessage response = new ProperMessage
                    {
                        messageType = MessageType.JoinAccept, 
                        messageData = JsonConvert.SerializeObject(player)
                    };
                    SendTest(JsonConvert.SerializeObject(response));
                    //response = new ProperMessage { messageType = MessageType.OpponentInfo, messageData = JsonConvert.SerializeObject(_players[0])};
                    //Send(JsonConvert.SerializeObject(response));
                    if (_players.Count == 2)
                    {
                        //response = new ProperMessage { messageType = MessageType.OpponentInfo, messageData = JsonConvert.SerializeObject(player)};
                        //Sessions.SendTo(JsonConvert.SerializeObject(response), _players[0].sessionId);
                    }
                }
                else
                {
                    ProperMessage response = new ProperMessage
                        {messageType = MessageType.JoinDeny, messageData = null};
                    SendTest(JsonConvert.SerializeObject(response));
                }
            }

            if (recmsg.messageType == MessageType.RoundPlay)
            {
                List<Player> _players = _game.getPlayers();
                Player player = JsonConvert.DeserializeObject<Player>(recmsg.messageData);
                if (VerifyRoundPlay(player))
                {
                    player.lockedIn = true;
                    var thisPlayerIndex = _players.IndexOf(_players.Find(p => p.sessionId == ID));
                    Console.WriteLine($"Index: {thisPlayerIndex}");
                    player.sessionId = ID;
                    _players[thisPlayerIndex] = player;
                    Console.WriteLine("Accepted Player Move");
                    Console.WriteLine($"Data: {e.Data}");
                    ProperMessage response = new ProperMessage { messageType = MessageType.RoundPlayAccept, messageData = null};
                    SendTest(JsonConvert.SerializeObject(response));
                }
                else
                {
                    Console.WriteLine("Denied Player Move");
                    Console.WriteLine($"Data: {e.Data}");
                    ProperMessage response = new ProperMessage { messageType = MessageType.RoundPlayDeny, messageData = null};
                    SendTest(JsonConvert.SerializeObject(response));
                }
                
                if (_players.Count == 2 && (_players[0].lockedIn && _players[1].lockedIn))
                {
                    var gameRound = DetermineRound(_players[0], _players[1]);
                    Console.WriteLine(JsonConvert.SerializeObject(gameRound));
                    ProperMessage response = new ProperMessage
                        {messageType = MessageType.RoundResult, messageData = JsonConvert.SerializeObject(gameRound)};
                    Sessions.Broadcast(JsonConvert.SerializeObject(response));
                    ResetRound();
                }
            }
        }

        private bool VerifyRoundPlay(Player player)
        {
                int energy = 7;

                for (int i = 0; i <= 4; i++)
                {
                    // Verify player action choices pt 1
                    ActionType actionType = player.actions[i];
                    if (i > 0)
                    {
                       ActionType prevActionType = player.actions[i - 1];
                       if (actionType == ActionType.HeavySwordS &&
                           prevActionType != (int) ActionType.HeavySwordH)
                       {
                           return false;
                       }
                    }

                    // Verify player action choices pt 2
                    if (i < 4)
                    {
                        ActionType nextActionType = player.actions[i + 1];
                        if (actionType == (int) ActionType.HeavySwordH &&
                            nextActionType != ActionType.HeavySwordS)
                        {
                            return false;
                        }
                    }
                    
                    // calculate energy usage
                    switch (actionType)
                    {
                        case ActionType.HeavySwordH:
                            energy -= 0;
                            break;
                        case ActionType.Shield:
                            energy -= 1;
                            break;
                        case ActionType.Sword:
                            energy -= 2;
                            break;
                        case ActionType.HeavySwordS:
                            energy -= 2;
                            break;
                    }
                }

                // verify energy usage
                if (energy < 0) return false;
                return true;
            
        }

        private int CalcDamage(ActionType attackAction, ActionType defendAction)
        {
            switch (attackAction)
            {
                case ActionType.Shield:
                    return 0;
                case ActionType.Sword:
                    if (defendAction == ActionType.Shield)
                        return 0;
                    else if (defendAction == ActionType.HeavySwordH)
                        return 2;
                    else
                        return 1;
                case ActionType.HeavySwordS:
                    if (defendAction == ActionType.Shield)
                        return 1;
                    else if (defendAction == ActionType.HeavySwordH)
                        return 3;
                    else
                        return 2;
                case ActionType.HeavySwordH:
                    return 0;
            }

            return 0;
        }
        private GameRound DetermineRound(Player p1, Player p2)
        {
            int p1D = 0;
            int p2D = 0;
            for (int i = 0; i <= 4; i++)
            {
                p1D += CalcDamage((ActionType)p1.actions[i], (ActionType)p2.actions[i]);
                p2D += CalcDamage((ActionType)p2.actions[i], (ActionType)p1.actions[i]);
            }

            int w = -1;
            if (p1D > p2D) w = 1;
            else if (p1D < p2D) w = 2;
            else w = 3;
            p1.lockedIn = false;
            p2.lockedIn = false;
            GameRound gameRound = new GameRound {player1 = p1, player2 = p2, player1Damnage = p1D, player2Damage = p2D, winner = w};
            return gameRound;
        }

        private void ResetRound()
        {
            var players = _game.getPlayers();
            ActionType[] defaultActions = new[] {ActionType.NullAction, ActionType.NullAction, ActionType.NullAction, ActionType.NullAction, ActionType.NullAction};
            var player1 = players[0];
            var player2 = players[1];
            player1.actions = defaultActions;
            player2.actions = defaultActions;
            _game.ResetPlayers();
            _game.AddPlayer(player1);
            _game.AddPlayer(player2);
        }

        private void SendTest(string toSend)
        {
            Console.WriteLine(toSend);
            Send(toSend);
        }
    }
}