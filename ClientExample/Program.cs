using System;
using System.Linq;
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
                Player me = new Player {sessionId = null, name = RandomString(8), avatar = 1, actions = new []{-1,-1,-1,-1,-1}};
                Player opponent;
                ProperMessage joinMessage = new ProperMessage {messageType = MessageType.Join, messageData = JsonConvert.SerializeObject(me)};
                ws.Send (JsonConvert.SerializeObject(joinMessage));
                
                Console.ReadKey (true);
            }
        }
    }
}