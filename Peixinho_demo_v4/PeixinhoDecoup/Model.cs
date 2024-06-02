using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace PeixinhoDecoup;


public interface IViewModel
{
    event EventHandler<Model.MatchFoundEventArgs> MatchChecked;
    void RequestPlayer1Hand(ref List<ArrayList> list);
    void checkmatch(string card);
    int GetPlayerScore(int playerId);
}
public class Model:IViewModel
{
    
    List<int> randomList = new List<int>();
    List<string> cardsNames=new List<string>();
    List<string> playerCards =new List<string>();
    private List<Player> players;
    
    public event EventHandler<MatchFoundEventArgs> MatchChecked;
    

    private Deck deck;
   
    
    public void StarGame()
    {
        deck = new Deck();
        players = new List<Player>();
        for (int i = 1; i <= 2; i++)
        {
            players.Add(new Player(i));
        }

        DealCards();
    }
    
    public void DealCards()
    {
        deck.Shuffle();
        for (int i = 0; i < 9; i++)
        {
            foreach (var player in players)
            {
                if (deck.GetCards().Count > 0)
                {
                    var card = deck.GetCards()[0];
                    player.AddCardToHand(card);
                    deck.GetCards().RemoveAt(0);
                }
            }
        }
    }

    public void RequestPlayer1Hand(ref List<ArrayList> list)
    {
        Console.WriteLine("4");
        if (players.Count > 0)
        {
            List<Card> p1 = new List<Card>(players[0].Hand);
            List<Card> p2 = new List<Card>(players[1].Hand);
            ArrayList p1ArrayList = new ArrayList(p1);
            ArrayList p2ArrayList = new ArrayList(p2);

            list = new List<ArrayList>();
            list.Add(p1ArrayList);
            list.Add(p2ArrayList);
        }
        else
        {
            list = new List<ArrayList>(); 
        }
    }

    public void checkmatch (string card) 
    {
        string[] namesplit = card.Split('_');

        string cardValue = namesplit[0];
            
        bool matchFound = players[1].Hand.Any(card => card.CardName.Split('_')[0] == cardValue);
        Console.WriteLine("M"+matchFound);
        OnMatchChecked(matchFound);
        if (matchFound)
        {
            Console.WriteLine("M2"+matchFound);
            
            MoveCards(players[1].Hand,players[0].Hand,cardValue);
        }
    }
    
    static void MoveCards(List<Card> source, List<Card> destination, string cardValue)
    {
        Card matchingCard = source.FirstOrDefault(card => card.CardName.Split('_')[0] == cardValue);
        
        if (matchingCard != null)
        {
            source.Remove(matchingCard);
            destination.Add(matchingCard);
        }
    }
    
    protected virtual void OnMatchChecked(bool matchFound)
    {
        MatchChecked?.Invoke(this, new MatchFoundEventArgs(matchFound));
    }
    
    public class MatchFoundEventArgs : EventArgs
    {
        public bool MatchFound { get; private set; }

        public MatchFoundEventArgs(bool matchFound)
        {
            MatchFound = matchFound;
        }
    }

    public int GetPlayerScore(int playerId)
    {
        foreach (var player in players)
        {
            if (player.ID == playerId)
            {
                return player.Points;
            }
        }
        return 0; 
    }

}