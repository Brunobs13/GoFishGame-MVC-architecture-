namespace GoFish.Core.Engine;

public sealed class TurnOutcome
{
    public required bool Accepted { get; init; }

    public required string Message { get; init; }

    public required bool MatchFound { get; init; }

    public required bool ExtraTurnGranted { get; init; }

    public required bool GameOver { get; init; }

    public required int CardsTransferred { get; init; }

    public required GameSnapshot Snapshot { get; init; }
}
