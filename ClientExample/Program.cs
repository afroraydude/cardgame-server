using System;
using System.Linq;
using System.Text.RegularExpressions;
using CardGameShared.Data;
using Newtonsoft.Json;
using WebSocketSharp;

namespace ClientExample
{
    class Program
    {
        static string RandomString (int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static void Main (string[] args)
        {
            using (var ws = new WebSocket ("ws://localhost:5001/DebugGame"))
            {
                ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine($"Received Message:\n{e.Data}");
                    ProperMessage recvmsg = JsonConvert.DeserializeObject<ProperMessage>(e.Data);
                    if (recvmsg.messageType == MessageType.JoinAccept)
                    {
                        Console.WriteLine("Join Accepted");
                    } else if (recvmsg.messageType == MessageType.JoinDeny)
                    {
                        Console.WriteLine("Join Denied");
                    }
                };
                ws.Connect ();
                Player me = new Player {sessionId = null, name = RandomString(8), avatar = Avatar.BaldGuy, 
                    actions = new[] {ActionType.NullAction, ActionType.NullAction, ActionType.NullAction, 
                        ActionType.NullAction, ActionType.NullAction}, lockedIn = false};
                ProperMessage joinMessage = new ProperMessage {messageType = MessageType.Join, messageData = JsonConvert.SerializeObject(me)};
                ws.Send (JsonConvert.SerializeObject(joinMessage));
                
                var actionsText = Console.ReadLine();
                me.actions = ActionResponse(actionsText);
                ProperMessage playMessage = new ProperMessage {messageType = MessageType.RoundPlay, messageData = JsonConvert.SerializeObject(me)};
                ws.Send(JsonConvert.SerializeObject(playMessage));
                Console.ReadKey (true);
            }
        }

        private static ActionType[] ActionResponse(string actions)
        {
            ActionType[] response = new[] {ActionType.NullAction, ActionType.NullAction, ActionType.NullAction, ActionType.NullAction, ActionType.NullAction};;
            actions = actions.ToLower();
            Regex rgx = new Regex("[^a-z]");
            actions = rgx.Replace(actions, "");
            char[] actionsCR = actions.ToCharArray();
            /*
             * Key for actions:
             * h: Heavy Sword H
             * s: Sword
             * w: Heavy Sword S
             * x: Shield
             */

            for (int i = 0; i <= 4; i++)
            {
                char action = actionsCR[i];

                switch (action)
                {
                    case 'h':
                        response[i] = ActionType.HeavySwordH;
                        break;
                    case 's':
                        response[i] = ActionType.Sword;
                        break;
                    case 'w':
                        response[i] = ActionType.HeavySwordS;
                        break;
                    case 'x':
                        response[i] = ActionType.Shield;
                        break;
                }
            }

            return response;
        }
    }
}