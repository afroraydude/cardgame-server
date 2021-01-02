namespace CardGameShared.Data
{
    public struct Player
    {
        public string sessionId { get; set; }
        public string name { get; set; }
        public int avatar { get; set; }
        public int[] actions { get; set; }
    }
}