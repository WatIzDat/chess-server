namespace ChessServer.Api.Domain.Game;

public enum GameResult
{
    None,
    Checkmate,
    Stalemate,
    DrawByRepetition,
    DrawByFiftyMoveRule,
    DrawByInsufficientMaterial
}