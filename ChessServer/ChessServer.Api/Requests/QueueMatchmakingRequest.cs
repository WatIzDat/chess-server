namespace ChessServer.Api.Requests;

public class QueueMatchmakingRequest
{
    public int InitialTimeSeconds { get; set; }
    public int IncrementTimeSeconds { get; set; }
    public bool UseDelay { get; set; }
}