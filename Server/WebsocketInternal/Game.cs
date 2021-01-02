using System.Collections.Generic;
using CardGameShared.Data;

namespace CardGameServer.WebsocketInternal
{
    public class Game
    {
        private List<Player> _players;

        public Game()
        {
            _players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
        }

        public List<Player> getPlayers()
        {
            return _players;
        }
    }
}