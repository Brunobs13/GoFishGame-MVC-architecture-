namespace GoFish.Core.Engine;

public sealed class GameMetrics
{
    public DateTimeOffset StartedAtUtc { get; private set; }

    public DateTimeOffset LastActionAtUtc { get; private set; }

    public int TurnsProcessed { get; private set; }

    public int TotalRequests { get; private set; }

    public int SuccessfulRequests { get; private set; }

    public int GoFishCount { get; private set; }

    public int CardsTransferred { get; private set; }

    public int BooksCompleted { get; private set; }

    public int CompletedGames { get; private set; }

    internal void MarkGameStarted()
    {
        StartedAtUtc = DateTimeOffset.UtcNow;
        LastActionAtUtc = StartedAtUtc;
        TurnsProcessed = 0;
        TotalRequests = 0;
        SuccessfulRequests = 0;
        GoFishCount = 0;
        CardsTransferred = 0;
        BooksCompleted = 0;
    }

    internal void RecordTurn()
    {
        TurnsProcessed++;
        LastActionAtUtc = DateTimeOffset.UtcNow;
    }

    internal void RecordRequest()
    {
        TotalRequests++;
        LastActionAtUtc = DateTimeOffset.UtcNow;
    }

    internal void RecordSuccessfulRequest(int transferredCards)
    {
        SuccessfulRequests++;
        CardsTransferred += transferredCards;
        LastActionAtUtc = DateTimeOffset.UtcNow;
    }

    internal void RecordGoFish()
    {
        GoFishCount++;
        LastActionAtUtc = DateTimeOffset.UtcNow;
    }

    internal void RecordBook()
    {
        BooksCompleted++;
        LastActionAtUtc = DateTimeOffset.UtcNow;
    }

    internal void RecordGameCompleted()
    {
        CompletedGames++;
        LastActionAtUtc = DateTimeOffset.UtcNow;
    }

    public GameMetrics Snapshot() => new()
    {
        StartedAtUtc = StartedAtUtc,
        LastActionAtUtc = LastActionAtUtc,
        TurnsProcessed = TurnsProcessed,
        TotalRequests = TotalRequests,
        SuccessfulRequests = SuccessfulRequests,
        GoFishCount = GoFishCount,
        CardsTransferred = CardsTransferred,
        BooksCompleted = BooksCompleted,
        CompletedGames = CompletedGames,
    };
}
