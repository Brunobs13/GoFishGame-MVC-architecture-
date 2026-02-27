const stateEls = {
  feedback: document.getElementById("feedback"),
  statusValue: document.getElementById("statusValue"),
  turnValue: document.getElementById("turnValue"),
  deckValue: document.getElementById("deckValue"),
  scoreValue: document.getElementById("scoreValue"),
  rankSelect: document.getElementById("rankSelect"),
  askBtn: document.getElementById("askBtn"),
  resetBtn: document.getElementById("resetBtn"),
  seedInput: document.getElementById("seedInput"),
  ruleHint: document.getElementById("ruleHint"),
  handCards: document.getElementById("handCards"),
  playerBooks: document.getElementById("playerBooks"),
  botBooks: document.getElementById("botBooks"),
  eventFeed: document.getElementById("eventFeed"),
  metricRequests: document.getElementById("metricRequests"),
  metricSuccess: document.getElementById("metricSuccess"),
  metricGoFish: document.getElementById("metricGoFish"),
  metricTurns: document.getElementById("metricTurns"),
  successMeter: document.getElementById("successMeter"),
  successRate: document.getElementById("successRate"),
};

const suitOrder = ["Clubs", "Diamonds", "Hearts", "Spades"];
let latestState = null;

async function api(path, options = {}) {
  const response = await fetch(path, {
    headers: {
      "Content-Type": "application/json",
      ...(options.headers || {}),
    },
    ...options,
  });

  let data = null;
  try {
    data = await response.json();
  } catch {
    data = null;
  }

  if (!response.ok) {
    const message = data?.message || `Request failed (${response.status})`;
    throw new Error(message);
  }

  return data;
}

function setFeedback(message, isError = false) {
  stateEls.feedback.textContent = message;
  stateEls.feedback.style.color = isError ? "#ff8f8f" : "#68cf8d";
}

function createEmptyChip(text) {
  const chip = document.createElement("li");
  chip.textContent = text;
  chip.style.opacity = "0.72";
  return chip;
}

function renderChipList(target, values) {
  target.innerHTML = "";
  if (!values.length) {
    target.appendChild(createEmptyChip("none"));
    return;
  }

  values.forEach((value) => {
    const item = document.createElement("li");
    item.textContent = value;
    target.appendChild(item);
  });
}

function renderHand(cards) {
  stateEls.handCards.innerHTML = "";

  if (!cards.length) {
    const placeholder = document.createElement("div");
    placeholder.className = "play-card";
    placeholder.innerHTML = `<small>No cards in hand</small><div class="rank">-</div><span class="suit">wait</span>`;
    stateEls.handCards.appendChild(placeholder);
    return;
  }

  const sorted = [...cards].sort((a, b) => {
    const suitGap = suitOrder.indexOf(a.suit) - suitOrder.indexOf(b.suit);
    if (suitGap !== 0) {
      return suitGap;
    }
    return a.rank.localeCompare(b.rank);
  });

  sorted.forEach((card) => {
    const cardEl = document.createElement("article");
    cardEl.className = "play-card";
    cardEl.innerHTML = `
      <small>${card.suit}</small>
      <div class="rank">${card.label.split("-")[0]}</div>
      <span class="suit">${card.suit.slice(0, 3)}</span>
    `;
    stateEls.handCards.appendChild(cardEl);
  });
}

function renderEvents(events) {
  stateEls.eventFeed.innerHTML = "";
  if (!events.length) {
    const li = document.createElement("li");
    li.textContent = "No events yet.";
    stateEls.eventFeed.appendChild(li);
    return;
  }

  events.forEach((entry) => {
    const li = document.createElement("li");
    li.textContent = entry;
    stateEls.eventFeed.appendChild(li);
  });
}

function updateAskOptions(hand) {
  const uniqueRanks = [...new Set(hand.map((card) => card.rank))];
  stateEls.rankSelect.innerHTML = "";

  if (!uniqueRanks.length) {
    const option = document.createElement("option");
    option.value = "";
    option.textContent = "No valid rank";
    stateEls.rankSelect.appendChild(option);
    stateEls.askBtn.disabled = true;
    return;
  }

  uniqueRanks.forEach((rank) => {
    const option = document.createElement("option");
    option.value = rank;
    option.textContent = rank;
    stateEls.rankSelect.appendChild(option);
  });

  const canPlay = !latestState?.isGameOver && latestState?.isHumanTurn;
  stateEls.askBtn.disabled = !canPlay;
}

function renderState(state) {
  latestState = state;

  stateEls.statusValue.textContent = state.status;
  stateEls.turnValue.textContent = state.isHumanTurn ? "Player" : "Dealer AI";
  stateEls.deckValue.textContent = String(state.deckCount);
  stateEls.scoreValue.textContent = `${state.humanScore} - ${state.botScore}`;
  stateEls.ruleHint.textContent = state.isHumanTurn
    ? "Only ranks in your hand can be requested."
    : "Dealer AI is processing. Wait for your turn.";

  if (state.isGameOver) {
    const winner = state.winner ? ` Winner: ${state.winner}.` : "";
    setFeedback(`Match completed.${winner}`);
  }

  renderHand(state.humanHand || []);
  renderChipList(stateEls.playerBooks, state.humanBooks || []);
  renderChipList(stateEls.botBooks, state.botBooks || []);
  renderEvents(state.recentEvents || []);
  updateAskOptions(state.humanHand || []);
}

function renderMetrics(metrics) {
  stateEls.metricRequests.textContent = String(metrics.totalRequests);
  stateEls.metricSuccess.textContent = String(metrics.successfulRequests);
  stateEls.metricGoFish.textContent = String(metrics.goFishCount);
  stateEls.metricTurns.textContent = String(metrics.turnsProcessed);

  const successRate = metrics.totalRequests > 0
    ? (metrics.successfulRequests / metrics.totalRequests) * 100
    : 0;

  stateEls.successRate.textContent = `${successRate.toFixed(1)}%`;
  stateEls.successMeter.style.width = `${Math.min(Math.max(successRate, 0), 100)}%`;
}

async function refreshState() {
  const state = await api("/api/game/state");
  renderState(state);
}

async function refreshMetrics() {
  const metrics = await api("/api/game/metrics");
  renderMetrics(metrics);
}

async function handleAsk() {
  try {
    const rank = stateEls.rankSelect.value;
    if (!rank) {
      setFeedback("Choose a valid rank before asking.", true);
      return;
    }

    stateEls.askBtn.disabled = true;
    const result = await api("/api/game/player/ask", {
      method: "POST",
      body: JSON.stringify({ rank }),
    });

    setFeedback(result.message, !result.accepted);
    renderState(result.state);
    await refreshMetrics();
  } catch (error) {
    setFeedback(error.message || "Failed to process ask action.", true);
  } finally {
    if (latestState) {
      stateEls.askBtn.disabled = !(latestState.isHumanTurn && !latestState.isGameOver);
    }
  }
}

async function handleReset() {
  try {
    stateEls.resetBtn.disabled = true;
    const rawSeed = stateEls.seedInput.value.trim();
    const seed = rawSeed === "" ? null : Number(rawSeed);

    const payload = Number.isFinite(seed) ? { seed } : {};
    const state = await api("/api/game/reset", {
      method: "POST",
      body: JSON.stringify(payload),
    });

    renderState(state);
    await refreshMetrics();
    setFeedback("New match started.");
  } catch (error) {
    setFeedback(error.message || "Failed to reset game.", true);
  } finally {
    stateEls.resetBtn.disabled = false;
  }
}

async function boot() {
  try {
    await refreshState();
    await refreshMetrics();
    setFeedback("Dashboard connected.");
  } catch (error) {
    setFeedback(error.message || "Failed to initialize dashboard.", true);
  }

  setInterval(async () => {
    try {
      await refreshState();
      await refreshMetrics();
    } catch {
      // Keep UI responsive during intermittent API failures.
    }
  }, 4000);
}

stateEls.askBtn.addEventListener("click", handleAsk);
stateEls.resetBtn.addEventListener("click", handleReset);

boot();
