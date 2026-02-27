using GoFish.Core.Domain;

namespace GoFish.Core.Engine;

internal sealed class PlayerState
{
    public PlayerState(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public List<Card> Hand { get; } = [];

    public HashSet<CardRank> Books { get; } = [];

    public int Score => Books.Count;
}
