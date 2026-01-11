using System.Diagnostics;
using ChessServer.Api.Database;
using ChessServer.Api.Domain.Game;

namespace ChessServer.Api.Domain.Match;

public class Match
{
    public Match()
    {
    }

    public Match(ApplicationUser initialUser, double initialTimeSeconds, string fen = Fen.InitialPosition)
        : this([new MatchConnection(initialUser, MatchPlayerType.WhitePlayer)], initialTimeSeconds, fen)
    {
    }
    // {
    //     Connections =
    //     [
    //         new MatchConnection(initialUser, MatchPlayerType.WhitePlayer)
    //     ];
    //
    //     Board = Fen.CreateBoardFromFen(fen);
    //
    //     PositionKeyList = [Board.GetPositionKey()];
    //     
    //     //LastTurnStartTimestamp = Stopwatch.GetTimestamp();
    //
    //     WhiteTimeRemaining = (long)(initialTimeSeconds * Stopwatch.Frequency);
    //     BlackTimeRemaining = (long)(initialTimeSeconds * Stopwatch.Frequency);
    // }

    public Match(MatchConnection[] initialConnections, double initialTimeSeconds, string fen = Fen.InitialPosition)
    {
        Connections = initialConnections.ToList();

        Board = Fen.CreateBoardFromFen(fen);

        PositionKeyList = [Board.GetPositionKey()];
        
        LastTurnStartTimestamp = Stopwatch.GetTimestamp();

        WhiteTimeRemaining = (long)(initialTimeSeconds * Stopwatch.Frequency);
        BlackTimeRemaining = (long)(initialTimeSeconds * Stopwatch.Frequency);
    }
    
    public Guid Id { get; init; } = Guid.NewGuid();
    //public List<ApplicationUser> ConnectedUsers { get; init; }
    public List<MatchConnection> Connections { get; } = [];
    public Board Board { get; init; }
    public GameResult Result { get; set; } = GameResult.None;
    public List<string> PositionKeyList { get; set; } = [];
    public long LastTurnStartTimestamp { get; set; }
    public long WhiteTimeRemaining { get; set; }
    public long BlackTimeRemaining { get; set; }
}