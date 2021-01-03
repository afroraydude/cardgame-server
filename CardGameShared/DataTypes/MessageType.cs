namespace CardGameShared.Data
{
    public enum MessageType
    {
        Join,
        JoinAccept,
        JoinDeny,
        RoundPlay,
        RoundResult,
        RoundPlayAccept,
        RoundPlayDeny,
        Leave,
        OpponentInfo,
        CreateAccept,
        Create
    }
}