using System;
using System.Collections.Generic;

namespace PeixinhoDecoup;


public class Player
{
    public List<Card> Hand { get; private set; }
    public int ID { get; private set; }
    public int Points { get; set; }

    public Player(int id)
    {
        ID = id;
        Hand = new List<Card>();
        Points = 0;
    }

    public void AddCardToHand(Card card)
    {
        Hand.Add(card);
    }

    public void ShowHand()
    {
        Console.WriteLine($"Player {ID}'s Hand:");
        foreach (var card in Hand)
        {
            Console.WriteLine(card);
        }
    }
}
