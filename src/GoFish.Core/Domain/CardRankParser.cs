using System.Globalization;

namespace GoFish.Core.Domain;

public static class CardRankParser
{
    private static readonly Dictionary<string, CardRank> RankByToken =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["A"] = CardRank.Ace,
            ["ACE"] = CardRank.Ace,
            ["1"] = CardRank.Ace,
            ["2"] = CardRank.Two,
            ["3"] = CardRank.Three,
            ["4"] = CardRank.Four,
            ["5"] = CardRank.Five,
            ["6"] = CardRank.Six,
            ["7"] = CardRank.Seven,
            ["8"] = CardRank.Eight,
            ["9"] = CardRank.Nine,
            ["10"] = CardRank.Ten,
            ["J"] = CardRank.Jack,
            ["JACK"] = CardRank.Jack,
            ["11"] = CardRank.Jack,
            ["Q"] = CardRank.Queen,
            ["QUEEN"] = CardRank.Queen,
            ["12"] = CardRank.Queen,
            ["K"] = CardRank.King,
            ["KING"] = CardRank.King,
            ["13"] = CardRank.King,
        };

    public static IReadOnlyList<string> SupportedTokens { get; } =
    [
        "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K",
    ];

    public static bool TryParse(string? input, out CardRank rank)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            rank = default;
            return false;
        }

        var token = input.Trim();

        if (RankByToken.TryGetValue(token, out rank))
        {
            return true;
        }

        if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric) &&
            Enum.IsDefined(typeof(CardRank), numeric))
        {
            rank = (CardRank)numeric;
            return true;
        }

        return false;
    }
}

public static class CardRankExtensions
{
    public static string ToShortCode(this CardRank rank) => rank switch
    {
        CardRank.Ace => "A",
        CardRank.Two => "2",
        CardRank.Three => "3",
        CardRank.Four => "4",
        CardRank.Five => "5",
        CardRank.Six => "6",
        CardRank.Seven => "7",
        CardRank.Eight => "8",
        CardRank.Nine => "9",
        CardRank.Ten => "10",
        CardRank.Jack => "J",
        CardRank.Queen => "Q",
        CardRank.King => "K",
        _ => ((int)rank).ToString(CultureInfo.InvariantCulture),
    };

    public static string ToDisplayName(this CardRank rank) => rank switch
    {
        CardRank.Ace => "Ace",
        CardRank.Two => "2",
        CardRank.Three => "3",
        CardRank.Four => "4",
        CardRank.Five => "5",
        CardRank.Six => "6",
        CardRank.Seven => "7",
        CardRank.Eight => "8",
        CardRank.Nine => "9",
        CardRank.Ten => "10",
        CardRank.Jack => "Jack",
        CardRank.Queen => "Queen",
        CardRank.King => "King",
        _ => rank.ToString(),
    };
}
