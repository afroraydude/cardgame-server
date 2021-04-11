namespace CardGameShared.Data
{
    public struct Player
    {
        public string sessionId { get; set; }
        public string name { get; set; }
        public Avatar avatar { get; set; }
        public ActionType[] actions { get; set; }
        public bool lockedIn { get; set; }
    }
}