using GoFish.Core.Domain;
using GoFish.Core.Engine;

namespace GoFish.Core.Tests;

public sealed class GoFishEngineTests
{
    [Fact]
    public void StartNewGame_DealsOpeningHandsAndDeck()
    {
        var engine = new GoFishGameEngine(seed: 42);

        var snapshot = engine.StartNewGame();

        Assert.Equal(GoFishGameEngine.InitialHandSize, snapshot.HumanHand.Count);
        Assert.Equal(GoFishGameEngine.InitialHandSize, snapshot.BotHandCount);
        Assert.Equal(52 - (GoFishGameEngine.InitialHandSize * 2), snapshot.DeckCount);
        Assert.False(snapshot.IsGameOver);
    }

    [Fact]
    public void PlayerAsk_WhenOpponentHasCards_TransfersAndKeepsTurn()
    {
        var engine = new GoFishGameEngine();
        engine.SetStateForTesting(
            deckCards: [],
            humanCards:
            [
                NewCard(CardRank.Ace, CardSuit.Clubs),
            ],
            botCards:
            [
                NewCard(CardRank.Ace, CardSuit.Hearts),
                NewCard(CardRank.Five, CardSuit.Spades),
            ],
            isHumanTurn: true);

        var outcome = engine.PlayerAsk(CardRank.Ace);

        Assert.True(outcome.Accepted);
        Assert.True(outcome.MatchFound);
        Assert.True(outcome.ExtraTurnGranted);
        Assert.Equal(1, outcome.CardsTransferred);
        Assert.Equal(2, outcome.Snapshot.HumanHand.Count);
        Assert.Equal(1, outcome.Snapshot.BotHandCount);
    }

    [Fact]
    public void PlayerAsk_WhenNoMatch_DrawsCardAndLosesTurn()
    {
        var engine = new GoFishGameEngine();
        engine.SetStateForTesting(
            deckCards:
            [
                NewCard(CardRank.Nine, CardSuit.Clubs),
            ],
            humanCards:
            [
                NewCard(CardRank.Ace, CardSuit.Clubs),
            ],
            botCards: [],
            isHumanTurn: true);

        var outcome = engine.PlayerAsk(CardRank.Ace);

        Assert.True(outcome.Accepted);
        Assert.False(outcome.MatchFound);
        Assert.False(outcome.ExtraTurnGranted);
        Assert.Equal(0, outcome.CardsTransferred);
        Assert.Equal(0, outcome.Snapshot.DeckCount);
        Assert.Equal(2, outcome.Snapshot.HumanHand.Count);
    }

    [Fact]
    public void PlayerAsk_WhenCompletesBook_UpdatesScore()
    {
        var engine = new GoFishGameEngine();
        engine.SetStateForTesting(
            deckCards: [],
            humanCards:
            [
                NewCard(CardRank.King, CardSuit.Clubs),
                NewCard(CardRank.King, CardSuit.Diamonds),
                NewCard(CardRank.King, CardSuit.Hearts),
                            ],
            botCards:
            [
                NewCard(CardRank.King, CardSuit.Spades),
                NewCard(CardRank.Ace, CardSuit.Clubs),
            ],
            isHumanTurn: true);

        var outcome = engine.PlayerAsk(CardRank.King);

        Assert.True(outcome.Accepted);
        Assert.Contains(CardRank.King, outcome.Snapshot.HumanBooks);
        Assert.Single(outcome.Snapshot.HumanBooks);
    }

    [Fact]
    public void PlayerAsk_WhenRankNotInHand_ReturnsRejectedOutcome()
    {
        var engine = new GoFishGameEngine();
        engine.SetStateForTesting(
            deckCards: [],
            humanCards:
            [
                NewCard(CardRank.Ace, CardSuit.Clubs),
            ],
            botCards:
            [
                NewCard(CardRank.Queen, CardSuit.Hearts),
            ],
            isHumanTurn: true);

        var outcome = engine.PlayerAsk(CardRank.King);

        Assert.False(outcome.Accepted);
        Assert.Contains("Invalid request", outcome.Message);
    }

    private static Card NewCard(CardRank rank, CardSuit suit) => new(rank, suit);
}
