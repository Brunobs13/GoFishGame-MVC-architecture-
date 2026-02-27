using System;
using System.Collections.Generic;
using System.IO;

namespace PeixinhoDecoup;

public class Deck
{
    //private List<string> cards;
    private static List<Card> cards;
    private Random random = new Random();


    static Deck()
    {
        var contentFiles = Directory.GetFiles("Content/Cards", "*.xnb", SearchOption.AllDirectories);
        cards = new List<Card>();
        foreach (var file in contentFiles)
        {
            // Get the relative paths and remove the extension
            var relativePath = Path.GetRelativePath("Content/Cards", file);
            var cardName = Path.Combine(Path.GetDirectoryName(relativePath),
                Path.GetFileNameWithoutExtension(relativePath)).Replace('\\', '/');
            var fullName = Path.GetFileNameWithoutExtension(cardName);
            Console.WriteLine("here"+fullName);
            var cardParts = fullName.Split('_');
            if (cardParts.Length == 2)
            {
                var value = cardParts[0];
                var suit = cardParts[1];
                var card = new Card(value, suit, fullName);
                cards.Add(card);
                Console.WriteLine(card);
            }
        }
    }
    
    public void Shuffle()
    {
        Random rng = new Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public  List<Card> GetCards()
    {
        return cards;
    }
    
    public static int GetCardCount()
    {
        return cards.Count;
    }
}

