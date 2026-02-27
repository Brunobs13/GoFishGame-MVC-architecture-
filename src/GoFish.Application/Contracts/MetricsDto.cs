namespace GoFish.Application.Contracts;

public sealed class MetricsDto
{
    public required DateTimeOffset StartedAtUtc { get; init; }

    public required DateTimeOffset LastActionAtUtc { get; init; }

    public required int TurnsProcessed { get; init; }

    public required int TotalRequests { get; init; }

    public required int SuccessfulRequests { get; init; }

    public required int GoFishCount { get; init; }

    public required int CardsTransferred { get; init; }

    public required int BooksCompleted { get; init; }

    public required int CompletedGames { get; init; }
}
