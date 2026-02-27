using GoFish.Core.Domain;

namespace GoFish.Core.Engine;

public sealed class GameSnapshot
{
    public required bool IsHumanTurn { get; init; }

    public required bool IsGameOver { get; init; }

    public required string Status { get; init; }

    public required string? Winner { get; init; }

    public required int DeckCount { get; init; }

    public required IReadOnlyList<Card> HumanHand { get; init; }

    public required int BotHandCount { get; init; }

    public required IReadOnlyList<CardRank> HumanBooks { get; init; }

    public required IReadOnlyList<CardRank> BotBooks { get; init; }

    public required IReadOnlyList<string> RecentEvents { get; init; }
}
