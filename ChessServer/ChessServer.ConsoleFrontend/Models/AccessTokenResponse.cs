namespace ChessServer.ConsoleFrontend.Models;

public class AccessTokenResponse
{
    public required string TokenType { get; init; }
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required int ExpiresIn { get; init; }
}