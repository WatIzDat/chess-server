namespace ChessServer.Api.Domain.Match;

public enum MatchPlayerType
{
    WhitePlayer,
    BlackPlayer,
    Spectator
}

public static class MatchPlayerTypeExtensions
{
    public static bool IsPlayer(this MatchPlayerType matchPlayerType)
    {
        return matchPlayerType is MatchPlayerType.WhitePlayer or MatchPlayerType.BlackPlayer;
    }
}