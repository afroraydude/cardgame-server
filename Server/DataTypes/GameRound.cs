namespace CardGameShared.Data
{
    public struct GameRound
    {
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public int player1Damnage { get; set; }
        public int player2Damage { get; set; }
        public int winner { get; set; }

        public override string ToString()
        {
            return $"GameRound(" +
                   $"{player1.ToString()} v {player2.ToString()}" +
                   $")";
        }
    }
}