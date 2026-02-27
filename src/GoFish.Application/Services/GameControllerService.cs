using GoFish.Application.Contracts;
using GoFish.Core.Domain;
using GoFish.Core.Engine;

namespace GoFish.Application.Services;

public sealed class GameControllerService
{
    private readonly object _sync = new();
    private readonly GoFishGameEngine _engine;

    public GameControllerService()
    {
        _engine = new GoFishGameEngine();
        _engine.StartNewGame();
    }

    public GameStateDto GetState()
    {
        lock (_sync)
        {
            return MapState(_engine.GetSnapshot());
        }
    }

    public GameStateDto ResetGame(int? seed = null)
    {
        lock (_sync)
        {
            var snapshot = _engine.StartNewGame(seed);
            return MapState(snapshot);
        }
    }

    public TurnOutcomeDto AskPlayer(string? rankToken)
    {
        lock (_sync)
        {
            if (!CardRankParser.TryParse(rankToken, out var rank))
            {
                return new TurnOutcomeDto
                {
                    Accepted = false,
                    Message = "Invalid rank. Use A,2,3,4,5,6,7,8,9,10,J,Q,K.",
                    MatchFound = false,
                    ExtraTurnGranted = false,
                    CardsTransferred = 0,
                    GameOver = _engine.GetSnapshot().IsGameOver,
                    State = MapState(_engine.GetSnapshot()),
                };
            }

            var outcome = _engine.PlayerAsk(rank);
            return MapOutcome(outcome);
        }
    }

    public MetricsDto GetMetrics()
    {
        lock (_sync)
        {
            var metrics = _engine.GetMetrics();
            return new MetricsDto
            {
                StartedAtUtc = metrics.StartedAtUtc,
                LastActionAtUtc = metrics.LastActionAtUtc,
                TurnsProcessed = metrics.TurnsProcessed,
                TotalRequests = metrics.TotalRequests,
                SuccessfulRequests = metrics.SuccessfulRequests,
                GoFishCount = metrics.GoFishCount,
                CardsTransferred = metrics.CardsTransferred,
                BooksCompleted = metrics.BooksCompleted,
                CompletedGames = metrics.CompletedGames,
            };
        }
    }

    public IReadOnlyList<string> GetSupportedRanks() => CardRankParser.SupportedTokens;

    private static TurnOutcomeDto MapOutcome(TurnOutcome outcome) => new()
    {
        Accepted = outcome.Accepted,
        Message = outcome.Message,
        MatchFound = outcome.MatchFound,
        ExtraTurnGranted = outcome.ExtraTurnGranted,
        CardsTransferred = outcome.CardsTransferred,
        GameOver = outcome.GameOver,
        State = MapState(outcome.Snapshot),
    };

    private static GameStateDto MapState(GameSnapshot snapshot) => new()
    {
        IsHumanTurn = snapshot.IsHumanTurn,
        IsGameOver = snapshot.IsGameOver,
        Status = snapshot.Status,
        Winner = snapshot.Winner,
        DeckCount = snapshot.DeckCount,
        HumanScore = snapshot.HumanBooks.Count,
        BotScore = snapshot.BotBooks.Count,
        BotHandCount = snapshot.BotHandCount,
        HumanHand = snapshot.HumanHand
            .Select(MapCard)
            .ToList(),
        HumanBooks = snapshot.HumanBooks
            .Select(rank => rank.ToDisplayName())
            .ToList(),
        BotBooks = snapshot.BotBooks
            .Select(rank => rank.ToDisplayName())
            .ToList(),
        RecentEvents = snapshot.RecentEvents,
    };

    private static CardDto MapCard(Card card) => new()
    {
        Rank = card.Rank.ToDisplayName(),
        Suit = card.Suit.ToString(),
        Label = $"{card.Rank.ToShortCode()}-{card.Suit.ToString()[0]}",
        Code = $"{card.Rank.ToShortCode()}_{card.Suit}",
    };
}
