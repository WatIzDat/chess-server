using ChessServer.Api.Database;
using ChessServer.Api.Domain.Game;

namespace ChessServer.Api.Domain.Match;

public class Match
{
    public Match()
    {
    }

    public Match(ApplicationUser initialUser, string fen = Fen.InitialPosition)
    {
        Connections =
        [
            new MatchConnection(initialUser, MatchPlayerType.WhitePlayer)
        ];

        Board = Fen.CreateBoardFromFen(fen);

        PositionKeyList = [Board.GetPositionKey()];
    }
    
    public Guid Id { get; init; } = Guid.NewGuid();
    //public List<ApplicationUser> ConnectedUsers { get; init; }
    public List<MatchConnection> Connections { get; } = [];
    public Board Board { get; init; }
    public List<string> PositionKeyList { get; set; } = [];
}