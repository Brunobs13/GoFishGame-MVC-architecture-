using System;
using System.Collections.Generic;
using System.Linq;

namespace FishGame
{
    public class Program
    {
        private static List<string> previouslyRequestedCards = new List<string>();
        private static int playerPeixinhos = 0;
        private static int opponentPeixinhos = 0;

        public static void Main(string[] args)
        {
            Model model = new Model();

            // Request data from the model
            List<string> deck = null;
            List<string> userHand = null;
            List<string> opponentHand = null;
            model.RequestData(ref deck, ref userHand, ref opponentHand);

            // Game loop
            while (playerPeixinhos + opponentPeixinhos < 13 && (deck.Count > 0 || userHand.Count > 0 || opponentHand.Count > 0))
            {
                // Player's turn
                PlayerTurn(deck, userHand, opponentHand);

                // Check if game ended
                if (playerPeixinhos + opponentPeixinhos >= 13 || (deck.Count == 0 && userHand.Count == 0 && opponentHand.Count == 0)) break;

                // Opponent's turn
                OpponentTurn(deck, userHand, opponentHand);

                // Check if game ended
                if (playerPeixinhos + opponentPeixinhos >= 13 || (deck.Count == 0 && userHand.Count == 0 && opponentHand.Count == 0)) break;
            }

            // Determine and display the winner
            EndGame();
        }

        private static void DisplayCards(string title, List<string> cards)
        {
            Console.WriteLine($"\n{title}:");
            foreach (var card in SortCards(cards))
            {
                Console.WriteLine(card);
            }
        }

        private static List<string> SortCards(List<string> cards)
        {
            var order = new Dictionary<string, int>
            {
                { "Ace", 1 },
                { "2", 2 },
                { "3", 3 },
                { "4", 4 },
                { "5", 5 },
                { "6", 6 },
                { "7", 7 },
                { "8", 8 },
                { "9", 9 },
                { "10", 10 },
                { "Jack", 11 },
                { "Queen", 12 },
                { "King", 13 }
            };

            return cards.OrderBy(card => order[card.Split(' ')[0]]).ToList();
        }

        private static string GetCardValue(string option)
        {
            return option switch
            {
                "1" => "Ace",
                "2" => "2",
                "3" => "3",
                "4" => "4",
                "5" => "5",
                "6" => "6",
                "7" => "7",
                "8" => "8",
                "9" => "9",
                "10" => "10",
                "11" => "Jack",
                "12" => "Queen",
                "13" => "King",
                _ => null
            };
        }

        private static void PlayerTurn(List<string> deck, List<string> userHand, List<string> opponentHand)
        {
            bool successfulRequest;
            do
            {
                successfulRequest = false;
                DisplayGameState(deck, userHand);

                Console.WriteLine("\nYour turn. Request a card:");
                Console.WriteLine("Choose a card to request (1-13):");
                Console.WriteLine("1. Ace");
                Console.WriteLine("2. 2");
                Console.WriteLine("3. 3");
                Console.WriteLine("4. 4");
                Console.WriteLine("5. 5");
                Console.WriteLine("6. 6");
                Console.WriteLine("7. 7");
                Console.WriteLine("8. 8");
                Console.WriteLine("9. 9");
                Console.WriteLine("10. 10");
                Console.WriteLine("11. Jack");
                Console.WriteLine("12. Queen");
                Console.WriteLine("13. King");

                string option = Console.ReadLine();
                string cardValue = GetCardValue(option);

                if (!string.IsNullOrEmpty(cardValue))
                {
                    if (TransferCards(opponentHand, userHand, cardValue))
                    {
                        Console.WriteLine($"You requested the card: {cardValue}");
                        Console.WriteLine("You received the card(s) from the opponent.");
                        if (opponentHand.Count == 0 && deck.Count > 0)
                        {
                            Console.WriteLine("Opponent has no cards left. Drawing a card from the deck for the opponent...");
                            DrawCardFromDeck(deck, opponentHand);
                        }
                        successfulRequest = true;
                        if (CheckForPeixinho(userHand, ref playerPeixinhos))
                        {
                            Console.WriteLine("You formed a Peixinho!");
                            if (IsGameOver(deck, userHand, opponentHand)) return;
                            successfulRequest = true; // Play again if a Peixinho is formed
                        }
                    }
                    else
                    {
                        Console.WriteLine($"The opponent does not have any {cardValue} cards.");
                        if (deck.Count > 0)
                        {
                            Console.WriteLine("Drawing a card from the deck...");
                            DrawCardFromDeck(deck, userHand);
                            if (CheckForPeixinho(userHand, ref playerPeixinhos))
                            {
                                Console.WriteLine("You formed a Peixinho!");
                                if (IsGameOver(deck, userHand, opponentHand)) return;
                                successfulRequest = true; // Play again if a Peixinho is formed
                            }
                        }
                        else
                        {
                            Console.WriteLine("The deck is empty. No more cards to draw.");
                        }
                    }
                    previouslyRequestedCards.Add(cardValue);
                }
                else
                {
                    Console.WriteLine("Invalid option, try again.");
                }
            } while (successfulRequest);
        }

        private static void OpponentTurn(List<string> deck, List<string> userHand, List<string> opponentHand)
        {
            bool successfulRequest;
            do
            {
                successfulRequest = false;
                DisplayGameState(deck, userHand);

                string cardValue = GetOpponentCardRequest(opponentHand);

                if (!string.IsNullOrEmpty(cardValue))
                {
                    if (TransferCards(userHand, opponentHand, cardValue))
                    {
                        Console.WriteLine($"Opponent requested the card: {cardValue}");
                        Console.WriteLine("Opponent received the card(s) from you.");
                        if (userHand.Count == 0 && deck.Count > 0)
                        {
                            Console.WriteLine("Player has no cards left. Drawing a card from the deck for the player...");
                            DrawCardFromDeck(deck, userHand);
                        }
                        successfulRequest = true;
                        if (CheckForPeixinho(opponentHand, ref opponentPeixinhos))
                        {
                            Console.WriteLine("Opponent formed a Peixinho!");
                            if (IsGameOver(deck, userHand, opponentHand)) return;
                            successfulRequest = true; // Play again if a Peixinho is formed
                        }
                    }
                    else
                    {
                        Console.WriteLine($"You do not have any {cardValue} cards.");
                        if (deck.Count > 0)
                        {
                            Console.WriteLine("Opponent is drawing a card from the deck...");
                            DrawCardFromDeck(deck, opponentHand);
                            if
using System;
                            using System.Collections.Generic;
                            using System.Linq;

namespace FishGame
    {
        public class Program
        {
            private static List<string> previouslyRequestedCards = new List<string>();
            private static int playerPeixinhos = 0;
            private static int opponentPeixinhos = 0;

            public static void Main(string[] args)
            {
                Model model = new Model();

                // Request data from the model
                List<string> deck = null;
                List<string> userHand = null;
                List<string> opponentHand = null;
                model.RequestData(ref deck, ref userHand, ref opponentHand);

                // Game loop
                while (playerPeixinhos + opponentPeixinhos < 13 && (deck.Count > 0 || userHand.Count > 0 || opponentHand.Count > 0))
                {
                    // Player's turn
                    PlayerTurn(deck, userHand, opponentHand);

                    // Check if game ended
                    if (playerPeixinhos + opponentPeixinhos >= 13 || (deck.Count == 0 && userHand.Count == 0 && opponentHand.Count == 0)) break;

                    // Opponent's turn
                    OpponentTurn(deck, userHand, opponentHand);

                    // Check if game ended
                    if (playerPeixinhos + opponentPeixinhos >= 13 || (deck.Count == 0 && userHand.Count == 0 && opponentHand.Count == 0)) break;
                }

                // Determine and display the winner
                EndGame();
            }

            private static void DisplayCards(string title, List<string> cards)
            {
                Console.WriteLine($"\n{title}:");
                foreach (var card in SortCards(cards))
                {
                    Console.WriteLine(card);
                }
            }

            private static List<string> SortCards(List<string> cards)
            {
                var order = new Dictionary<string, int>
            {
                { "Ace", 1 },
                { "2", 2 },
                { "3", 3 },
                { "4", 4 },
                { "5", 5 },
                { "6", 6 },
                { "7", 7 },
                { "8", 8 },
                { "9", 9 },
                { "10", 10 },
                { "Jack", 11 },
                { "Queen", 12 },
                { "King", 13 }
            };

                return cards.OrderBy(card => order[card.Split(' ')[0]]).ToList();
            }

            private static string GetCardValue(string option)
            {
                return option switch
                {
                    "1" => "Ace",
                    "2" => "2",
                    "3" => "3",
                    "4" => "4",
                    "5" => "5",
                    "6" => "6",
                    "7" => "7",
                    "8" => "8",
                    "9" => "9",
                    "10" => "10",
                    "11" => "Jack",
                    "12" => "Queen",
                    "13" => "King",
                    _ => null
                };
            }

            private static void PlayerTurn(List<string> deck, List<string> userHand, List<string> opponentHand)
            {
                bool successfulRequest;
                do
                {
                    successfulRequest = false;
                    DisplayGameState(deck, userHand);

                    Console.WriteLine("\nYour turn. Request a card:");
                    Console.WriteLine("Choose a card to request (1-13):");
                    Console.WriteLine("1. Ace");
                    Console.WriteLine("2. 2");
                    Console.WriteLine("3. 3");
                    Console.WriteLine("4. 4");
                    Console.WriteLine("5. 5");
                    Console.WriteLine("6. 6");
                    Console.WriteLine("7. 7");
                    Console.WriteLine("8. 8");
                    Console.WriteLine("9. 9");
                    Console.WriteLine("10. 10");
                    Console.WriteLine("11. Jack");
                    Console.WriteLine("12. Queen");
                    Console.WriteLine("13. King");

                    string option = Console.ReadLine();
                    string cardValue = GetCardValue(option);

                    if (!string.IsNullOrEmpty(cardValue))
                    {
                        if (TransferCards(opponentHand, userHand, cardValue))
                        {
                            Console.WriteLine($"You requested the card: {cardValue}");
                            Console.WriteLine("You received the card(s) from the opponent.");
                            if (opponentHand.Count == 0 && deck.Count > 0)
                            {
                                Console.WriteLine("Opponent has no cards left. Drawing a card from the deck for the opponent...");
                                DrawCardFromDeck(deck, opponentHand);
                            }
                            successfulRequest = true;
                            if (CheckForPeixinho(userHand, ref playerPeixinhos))
                            {
                                Console.WriteLine("You formed a Peixinho!");
                                if (IsGameOver(deck, userHand, opponentHand)) return;
                                successfulRequest = true; // Play again if a Peixinho is formed
                            }
                        }
                        else
                        {
                            Console.WriteLine($"The opponent does not have any {cardValue} cards.");
                            if (deck.Count > 0)
                            {
                                Console.WriteLine("Drawing a card from the deck...");
                                DrawCardFromDeck(deck, userHand);
                                if (CheckForPeixinho(userHand, ref playerPeixinhos))
                                {
                                    Console.WriteLine("You formed a Peixinho!");
                                    if (IsGameOver(deck, userHand, opponentHand)) return;
                                    successfulRequest = true; // Play again if a Peixinho is formed
                                }
                            }
                            else
                            {
                                Console.WriteLine("The deck is empty. No more cards to draw.");
                            }
                        }
                        previouslyRequestedCards.Add(cardValue);
                    }
                    else
                    {
                        Console.WriteLine("Invalid option, try again.");
                    }
                } while (successfulRequest);
            }

            private static void OpponentTurn(List<string> deck, List<string> userHand, List<string> opponentHand)
            {
                bool successfulRequest;
                do
                {
                    successfulRequest = false;
                    DisplayGameState(deck, userHand);

                    string cardValue = GetOpponentCardRequest(opponentHand);

                    if (!string.IsNullOrEmpty(cardValue))
                    {
                        if (TransferCards(userHand, opponentHand, cardValue))
                        {
                            Console.WriteLine($"Opponent requested the card: {cardValue}");
                            Console.WriteLine("Opponent received the card(s) from you.");
                            if (userHand.Count == 0 && deck.Count > 0)
                            {
                                Console.WriteLine("Player has no cards left. Drawing a card from the deck for the player...");
                                DrawCardFromDeck(deck, userHand);
                            }
                            successfulRequest = true;
                            if (CheckForPeixinho(opponentHand, ref opponentPeixinhos))
                            {
                                Console.WriteLine("Opponent formed a Peixinho!");
                                if (IsGameOver(deck, userHand, opponentHand)) return;
                                successfulRequest = true; // Play again if a Peixinho is formed
                            }
                        }
                        else
                        {
                            Console.WriteLine($"You do not have any {cardValue} cards.");
                            if (deck.Count > 0)
                            {
                                Console.WriteLine("Opponent is drawing a card from the deck...");
                                DrawCardFromDeck(deck, opponentHand);
                                if
