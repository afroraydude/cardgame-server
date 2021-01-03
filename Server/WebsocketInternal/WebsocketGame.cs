using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
                        {messageType = MessageType.JoinAccept, messageData = _code};
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

            if (recmsg.messageType == MessageType.RoundPlay)
            {
                List<Player> _players = _game.getPlayers();
                Player player = JsonConvert.DeserializeObject<Player>(recmsg.messageData);
                if (VerifyRoundPlay(player))
                {
                    player.lockedIn = true;
                    var thisPlayerIndex = _players.IndexOf(_players.Find(player => player.sessionId == ID));
                    Console.WriteLine($"Index: {thisPlayerIndex}");
                    _players[thisPlayerIndex] = player;
                    Console.WriteLine("Accepted Player Move");
                    Console.WriteLine($"Data: {e.Data}");
                    ProperMessage response = new ProperMessage { messageType = MessageType.RoundPlayAccept, messageData = null};
                    Send(JsonConvert.SerializeObject(response));
                }
                else
                {
                    Console.WriteLine("Denied Player Move");
                    Console.WriteLine($"Data: {e.Data}");
                    ProperMessage response = new ProperMessage { messageType = MessageType.RoundPlayDeny, messageData = null};
                    Send(JsonConvert.SerializeObject(response));
                }

                if (_players[0].lockedIn && _players[1].lockedIn)
                {
                    var gameRound = DetermineRound(_players[0], _players[1]);
                    Console.WriteLine(JsonConvert.SerializeObject(gameRound));
                    ProperMessage response = new ProperMessage
                        {messageType = MessageType.RoundResult, messageData = JsonConvert.SerializeObject(gameRound)};
                    Sessions.SendTo(JsonConvert.SerializeObject(response), _players[0].sessionId);
                    Sessions.SendTo(JsonConvert.SerializeObject(response), _players[1].sessionId);
                }
            }
        }

        private bool VerifyRoundPlay(Player player)
        {
                int energy = 7;

                for (int i = 0; i <= 4; i++)
                {
                    // Verify player action choices pt 1
                    int actionType = player.actions[i];
                    if (i > 0)
                    {
                       int prevActionType = player.actions[i - 1];
                       if (actionType == (int) ActionTypes.HeavySwordS &&
                           prevActionType != (int) ActionTypes.HeavySwordH)
                       {
                           return false;
                       }
                    }

                    // Verify player action choices pt 2
                    if (i < 4)
                    {
                        int nextActionType = player.actions[i + 1];
                        if (actionType == (int) ActionTypes.HeavySwordH &&
                            nextActionType != (int) ActionTypes.HeavySwordS)
                        {
                            return false;
                        }
                    }
                    
                    // calculate energy usage
                    switch (actionType)
                    {
                        case (int) ActionTypes.HeavySwordH:
                            energy -= 0;
                            break;
                        case (int) ActionTypes.Shield:
                            energy -= 1;
                            break;
                        case (int) ActionTypes.Sword:
                            energy -= 2;
                            break;
                        case (int) ActionTypes.HeavySwordS:
                            energy -= 2;
                            break;
                    }
                }

                // verify energy usage
                if (energy < 0) return false;
                return true;
            
        }

        private int CalcDamage(ActionTypes attackAction, ActionTypes defendAction)
        {
            switch (attackAction)
            {
                case ActionTypes.Shield:
                    return 0;
                case ActionTypes.Sword:
                    if (defendAction == ActionTypes.Shield)
                        return 0;
                    else if (defendAction == ActionTypes.HeavySwordH)
                        return 2;
                    else
                        return 1;
                case ActionTypes.HeavySwordS:
                    if (defendAction == ActionTypes.Shield)
                        return 1;
                    else if (defendAction == ActionTypes.HeavySwordH)
                        return 3;
                    else
                        return 2;
                case ActionTypes.HeavySwordH:
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
                p1D += CalcDamage((ActionTypes)p1.actions[i], (ActionTypes)p2.actions[i]);
                p2D += CalcDamage((ActionTypes)p2.actions[i], (ActionTypes)p1.actions[i]);
            }

            int w = -1;
            if (p1D > p2D) w = 1;
            else if (p1D < p2D) w = 2;
            else w = 3;
            GameRound gameRound = new GameRound {player1 = p1, player2 = p2, player1Damnage = p1D, player2Damage = p2D, winner = w};
            return gameRound;
        }
    }
}