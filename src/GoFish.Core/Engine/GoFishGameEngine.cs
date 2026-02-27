using GoFish.Core.Domain;

namespace GoFish.Core.Engine;

public sealed class GoFishGameEngine
{
    public const int InitialHandSize = 7;
    public const int MaxEventEntries = 200;

    private readonly PlayerState _human = new("Player");
    private readonly PlayerState _bot = new("Dealer AI");
    private readonly List<Card> _deck = [];
    private readonly Queue<string> _eventLog = new();
    private readonly GameMetrics _metrics = new();

    private Random _random;
    private bool _isInitialized;
    private bool _isHumanTurn;
    private bool _isGameOver;
    private string? _winner;

    public GoFishGameEngine(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public GameSnapshot StartNewGame(int? seed = null)
    {
        if (seed.HasValue)
        {
            _random = new Random(seed.Value);
        }

        _human.Hand.Clear();
        _human.Books.Clear();
        _bot.Hand.Clear();
        _bot.Books.Clear();
        _deck.Clear();
        _eventLog.Clear();

        _deck.AddRange(CreateShuffledDeck());

        for (var i = 0; i < InitialHandSize; i++)
        {
            DrawCard(_human, null, "Player drew opening card");
            DrawCard(_bot, null, "Dealer AI drew opening card");
        }

        var initialEvents = new List<string>();
        ResolveBooks(_human, initialEvents);
        ResolveBooks(_bot, initialEvents);

        _isInitialized = true;
        _isHumanTurn = true;
        _isGameOver = false;
        _winner = null;
        _metrics.MarkGameStarted();

        initialEvents.Insert(0, "New game started.");
        EvaluateGameOver(initialEvents);
        AppendEvents(initialEvents);

        return GetSnapshot();
    }

    public GameSnapshot GetSnapshot(int eventLimit = 25)
    {
        var limit = Math.Clamp(eventLimit, 1, MaxEventEntries);
        var recentEvents = _eventLog.Reverse().Take(limit).Reverse().ToList();

        return new GameSnapshot
        {
            IsHumanTurn = _isHumanTurn,
            IsGameOver = _isGameOver,
            Status = BuildStatus(),
            Winner = _winner,
            DeckCount = _deck.Count,
            HumanHand = _human.Hand
                .OrderBy(card => card.Rank)
                .ThenBy(card => card.Suit)
                .ToList(),
            BotHandCount = _bot.Hand.Count,
            HumanBooks = _human.Books.OrderBy(rank => rank).ToList(),
            BotBooks = _bot.Books.OrderBy(rank => rank).ToList(),
            RecentEvents = recentEvents,
        };
    }

    public TurnOutcome PlayerAsk(CardRank requestedRank)
    {
        if (!_isInitialized)
        {
            return Reject("Game not initialized. Reset the game before playing.");
        }

        if (_isGameOver)
        {
            return Reject("Game has already finished. Reset to start a new match.");
        }

        if (!_isHumanTurn)
        {
            return Reject("It is not your turn yet.");
        }

        var turnEvents = new List<string>();
        _metrics.RecordTurn();

        EnsurePlayerCanPlay(_human, turnEvents);
        if (_human.Hand.Count == 0)
        {
            EvaluateGameOver(turnEvents);
            AppendEvents(turnEvents);
            return Reject("Player has no cards and the deck is empty. Reset game.");
        }

        if (!_human.Hand.Any(card => card.Rank == requestedRank))
        {
            return Reject("Invalid request: you can only ask for ranks present in your hand.");
        }

        var matchFound = ExecuteAsk(
            requester: _human,
            opponent: _bot,
            requestedRank,
            turnEvents,
            out var transferredCards,
            out var drewRequestedRank);

        var extraTurn = matchFound || drewRequestedRank;

        if (!extraTurn && !_isGameOver)
        {
            _isHumanTurn = false;
            turnEvents.Add("Turn passed to Dealer AI.");
            RunBotTurns(turnEvents);
        }
        else
        {
            _isHumanTurn = true;
        }

        EvaluateGameOver(turnEvents);
        AppendEvents(turnEvents);

        return new TurnOutcome
        {
            Accepted = true,
            Message = BuildTurnMessage(matchFound, drewRequestedRank, transferredCards),
            MatchFound = matchFound,
            ExtraTurnGranted = extraTurn,
            CardsTransferred = transferredCards,
            GameOver = _isGameOver,
            Snapshot = GetSnapshot(),
        };
    }

    public GameMetrics GetMetrics() => _metrics.Snapshot();

    internal void SetStateForTesting(
        IEnumerable<Card> deckCards,
        IEnumerable<Card> humanCards,
        IEnumerable<Card> botCards,
        bool isHumanTurn = true)
    {
        _deck.Clear();
        _deck.AddRange(deckCards);

        _human.Hand.Clear();
        _human.Hand.AddRange(humanCards);
        _human.Books.Clear();

        _bot.Hand.Clear();
        _bot.Hand.AddRange(botCards);
        _bot.Books.Clear();

        _eventLog.Clear();
        _winner = null;
        _isInitialized = true;
        _isHumanTurn = isHumanTurn;
        _isGameOver = false;
        _metrics.MarkGameStarted();

        var events = new List<string> { "Test state loaded." };
        ResolveBooks(_human, events);
        ResolveBooks(_bot, events);
        EvaluateGameOver(events);
        AppendEvents(events);
    }

    private TurnOutcome Reject(string message) => new()
    {
        Accepted = false,
        Message = message,
        MatchFound = false,
        ExtraTurnGranted = false,
        CardsTransferred = 0,
        GameOver = _isGameOver,
        Snapshot = _isInitialized ? GetSnapshot() : BuildEmptySnapshot(message),
    };

    private bool ExecuteAsk(
        PlayerState requester,
        PlayerState opponent,
        CardRank requestedRank,
        List<string> events,
        out int transferredCards,
        out bool drewRequestedRank)
    {
        _metrics.RecordRequest();

        var matchingCards = opponent.Hand
            .Where(card => card.Rank == requestedRank)
            .ToList();

        if (matchingCards.Count > 0)
        {
            foreach (var card in matchingCards)
            {
                opponent.Hand.Remove(card);
                requester.Hand.Add(card);
            }

            transferredCards = matchingCards.Count;
            drewRequestedRank = false;
            _metrics.RecordSuccessfulRequest(transferredCards);

            events.Add($"{requester.Name} requested {requestedRank.ToDisplayName()} and received {transferredCards} card(s).");
            ResolveBooks(requester, events);
            EnsurePlayerCanPlay(opponent, events);

            return true;
        }

        transferredCards = 0;
        _metrics.RecordGoFish();

        events.Add($"{requester.Name} requested {requestedRank.ToDisplayName()} and went fishing.");
        var drawnCard = DrawCard(requester, events, $"{requester.Name} drew from the deck");
        drewRequestedRank = drawnCard is not null && drawnCard.Rank == requestedRank;

        if (drewRequestedRank)
        {
            events.Add($"{requester.Name} drew the requested rank and keeps the turn.");
        }

        ResolveBooks(requester, events);
        return false;
    }

    private void RunBotTurns(List<string> events)
    {
        var safetyCounter = 0;

        while (!_isGameOver && !_isHumanTurn && safetyCounter < 32)
        {
            safetyCounter++;
            _metrics.RecordTurn();

            EnsurePlayerCanPlay(_bot, events);
            if (_bot.Hand.Count == 0)
            {
                _isHumanTurn = true;
                events.Add("Dealer AI cannot continue. Turn returned to player.");
                return;
            }

            var requestedRank = ChooseBotRank();
            var matchFound = ExecuteAsk(
                requester: _bot,
                opponent: _human,
                requestedRank,
                events,
                out _,
                out var drewRequestedRank);

            var extraTurn = matchFound || drewRequestedRank;
            if (!extraTurn)
            {
                _isHumanTurn = true;
                events.Add("Your turn.");
            }
            else
            {
                _isHumanTurn = false;
                events.Add("Dealer AI keeps the turn.");
            }

            EvaluateGameOver(events);
        }
    }

    private void EnsurePlayerCanPlay(PlayerState player, List<string> events)
    {
        if (player.Hand.Count == 0 && _deck.Count > 0)
        {
            DrawCard(player, events, $"{player.Name} had no cards and drew from the deck");
            ResolveBooks(player, events);
        }
    }

    private Card? DrawCard(PlayerState player, List<string>? events, string reason)
    {
        if (_deck.Count == 0)
        {
            events?.Add($"{reason}: deck is empty.");
            return null;
        }

        var topCardIndex = _deck.Count - 1;
        var card = _deck[topCardIndex];
        _deck.RemoveAt(topCardIndex);
        player.Hand.Add(card);

        events?.Add($"{reason}.");
        return card;
    }

    private void ResolveBooks(PlayerState player, List<string> events)
    {
        var completedRanks = player.Hand
            .GroupBy(card => card.Rank)
            .Where(group => group.Count() == 4)
            .Select(group => group.Key)
            .ToList();

        foreach (var rank in completedRanks)
        {
            player.Hand.RemoveAll(card => card.Rank == rank);
            if (player.Books.Add(rank))
            {
                _metrics.RecordBook();
                events.Add($"{player.Name} completed a book of {rank.ToDisplayName()}.");
            }
        }
    }

    private void EvaluateGameOver(List<string> events)
    {
        if (_isGameOver)
        {
            return;
        }

        var allBooksCompleted = _human.Books.Count + _bot.Books.Count == 13;
        var noCardsLeft = _deck.Count == 0 && _human.Hand.Count == 0 && _bot.Hand.Count == 0;

        if (!allBooksCompleted && !noCardsLeft)
        {
            return;
        }

        _isGameOver = true;
        _winner = DetermineWinner();
        _metrics.RecordGameCompleted();
        events.Add($"Game over. Winner: {_winner}.");
    }

    private string DetermineWinner()
    {
        if (_human.Score == _bot.Score)
        {
            return "Draw";
        }

        return _human.Score > _bot.Score ? _human.Name : _bot.Name;
    }

    private CardRank ChooseBotRank()
    {
        var groups = _bot.Hand
            .GroupBy(card => card.Rank)
            .Select(group => new { Rank = group.Key, Count = group.Count() })
            .OrderByDescending(group => group.Count)
            .ToList();

        var topFrequency = groups.First().Count;
        var candidates = groups
            .Where(group => group.Count == topFrequency)
            .Select(group => group.Rank)
            .ToList();

        return candidates[_random.Next(candidates.Count)];
    }

    private List<Card> CreateShuffledDeck()
    {
        var deck = new List<Card>(52);
        foreach (var suit in Enum.GetValues<CardSuit>())
        {
            foreach (var rank in Enum.GetValues<CardRank>())
            {
                deck.Add(new Card(rank, suit));
            }
        }

        for (var i = deck.Count - 1; i > 0; i--)
        {
            var swapIndex = _random.Next(i + 1);
            (deck[i], deck[swapIndex]) = (deck[swapIndex], deck[i]);
        }

        return deck;
    }

    private void AppendEvents(IEnumerable<string> events)
    {
        foreach (var entry in events.Where(entry => !string.IsNullOrWhiteSpace(entry)))
        {
            var formatted = $"[{DateTimeOffset.UtcNow:HH:mm:ss}] {entry}";
            _eventLog.Enqueue(formatted);
            while (_eventLog.Count > MaxEventEntries)
            {
                _eventLog.Dequeue();
            }
        }
    }

    private string BuildTurnMessage(bool matchFound, bool drewRequestedRank, int transferredCards)
    {
        if (matchFound)
        {
            return $"Match found. You received {transferredCards} card(s).";
        }

        if (drewRequestedRank)
        {
            return "Go Fish succeeded: you drew the requested rank and keep the turn.";
        }

        return "Go Fish: no match found. Turn passed to Dealer AI.";
    }

    private string BuildStatus()
    {
        if (!_isInitialized)
        {
            return "Not started";
        }

        if (_isGameOver)
        {
            return $"Completed ({_winner})";
        }

        return _isHumanTurn ? "Player turn" : "Dealer AI turn";
    }

    private static GameSnapshot BuildEmptySnapshot(string status) => new()
    {
        IsHumanTurn = true,
        IsGameOver = false,
        Status = status,
        Winner = null,
        DeckCount = 0,
        HumanHand = [],
        BotHandCount = 0,
        HumanBooks = [],
        BotBooks = [],
        RecentEvents = [],
    };
}
