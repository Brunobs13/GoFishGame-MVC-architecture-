namespace PeixinhoDecoup;


public class Card
{
    public string Value { get; set; }
    public string Suit { get; set; }
    public string CardName { get; set; }

    public Card(string value, string suit, string cardName)
    {
        Value = value;
        Suit = suit;
        CardName = cardName;
    }

    public override string ToString()
    {
        return $"{Value} of {Suit}";
    }
}