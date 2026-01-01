namespace ChessServer.Api.Domain.Match;

public record TimeControl
{
    public TimeControl()
    {
    }
    
    public TimeControl(TimeSpan initialTime, TimeSpan incrementTime, bool useDelay = false)
    {
        InitialTime = initialTime;
        IncrementTime = incrementTime;
        UseDelay = useDelay;
    }
    
    public Guid Id { get; init; } = Guid.NewGuid();
    public TimeSpan InitialTime { get; set; }
    public TimeSpan IncrementTime { get; set; }
    public bool UseDelay { get; set; }
}