using System;
using System.Collections.Generic;

namespace PeixinhoDecoup
{
    public class Model
    {
        private List<string> deck = new List<string>();
        private List<string> userHand = new List<string>();
        private List<string> opponentHand = new List<string>();
        private ModelLog modelLog = new ModelLog();

        public Model()
        {
            try
            {
                InitializeDeck();
                ShuffleDeck();
                DealCards();
            }
            catch (Exception ex)
            {
                modelLog.LogError("Erro ao inicializar o jogo: " + ex.Message);
            }
        }

        private void InitializeDeck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    deck.Add($"{value} of {suit}");
                }
            }
        }

        private void ShuffleDeck()
        {
            Random random = new Random();
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                string temp = deck[i];
                deck[i] = deck[swapIndex];
                deck[swapIndex] = temp;
            }
        }

        private void DealCards()
        {
            for (int i = 0; i < 8; i++)  // Assuming each player gets 8 cards
            {
                userHand.Add(deck[i]);
                opponentHand.Add(deck[i + 8]);
            }
            deck.RemoveRange(0, 16);  // Removing the dealt cards from the deck
        }

        public void RequestData(ref List<string> deckRef, ref List<string> userHandRef, ref List<string> opponentHandRef)
        {
            deckRef = new List<string>(deck);
            userHandRef = new List<string>(userHand);
            opponentHandRef = new List<string>(opponentHand);
        }
    }

    public class ModelLog
    {
        private string log = "";

        public void LogError(string message)
        {
            log += DateTime.Now + ": " + message + "\n";
        }

        public string GetLog()
        {
            return log;
        }
    }
}
