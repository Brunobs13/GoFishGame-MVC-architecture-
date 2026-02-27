namespace GoFish.Application.Contracts;

public sealed class GameStateDto
{
    public required bool IsHumanTurn { get; init; }

    public required bool IsGameOver { get; init; }

    public required string Status { get; init; }

    public required string? Winner { get; init; }

    public required int DeckCount { get; init; }

    public required int HumanScore { get; init; }

    public required int BotScore { get; init; }

    public required int BotHandCount { get; init; }

    public required IReadOnlyList<CardDto> HumanHand { get; init; }

    public required IReadOnlyList<string> HumanBooks { get; init; }

    public required IReadOnlyList<string> BotBooks { get; init; }

    public required IReadOnlyList<string> RecentEvents { get; init; }
}
