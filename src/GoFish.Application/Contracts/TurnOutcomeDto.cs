namespace GoFish.Application.Contracts;

public sealed class TurnOutcomeDto
{
    public required bool Accepted { get; init; }

    public required string Message { get; init; }

    public required bool MatchFound { get; init; }

    public required bool ExtraTurnGranted { get; init; }

    public required int CardsTransferred { get; init; }

    public required bool GameOver { get; init; }

    public required GameStateDto State { get; init; }
}
