namespace GoFish.Application.Contracts;

public sealed class CardDto
{
    public required string Rank { get; init; }

    public required string Suit { get; init; }

    public required string Label { get; init; }

    public required string Code { get; init; }
}
