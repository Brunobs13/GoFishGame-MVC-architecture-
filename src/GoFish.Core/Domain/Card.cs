namespace GoFish.Core.Domain;

public sealed record Card(CardRank Rank, CardSuit Suit)
{
    public override string ToString() => $"{Rank.ToShortCode()} of {Suit}";
}
