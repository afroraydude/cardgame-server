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
        PlayerLeave,
        GameReset,
        ReadyForNewRound,
        OpponentInfo,
        CreateAccept,
        Create
    }
}