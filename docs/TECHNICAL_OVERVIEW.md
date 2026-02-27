# Technical Overview - GoFish Control Deck

## 1) Deep Architectural Explanation

### End-to-End Flow
1. Client submits gameplay command from dashboard (`POST /api/game/player/ask`).
2. API forwards request to `GameControllerService`.
3. Service validates input rank and orchestrates engine call.
4. `GoFishGameEngine` mutates state, applies rules, emits events, updates metrics.
5. API returns structured outcome + current snapshot.
6. Dashboard refreshes state and metrics panels.

### Design Decisions
- Domain logic isolated from transport layer.
- Controller/service layer centralizes synchronization and DTO mapping.
- API remains thin to reduce coupling.
- Frontend is static and deployable with API container.

### Trade-offs
- In-memory state keeps latency and complexity low but is not durable.
- Minimal API improves speed of delivery but has fewer built-in conventions than full MVC controllers.
- Single-process runtime is simple but not horizontally session-aware.

### Alternatives Considered
- Full ASP.NET MVC with Razor pages.
- Persisted state using Redis/PostgreSQL.
- WebSocket event push instead of polling.

## 2) Junior Interview Questions

1. What is MVC in this project?
Answer: Model is `GoFish.Core`, Controller is `GoFish.Application`, View is dashboard + API response rendering.

2. Why separate `Core` and `Application` projects?
Answer: To keep game rules independent from HTTP and UI concerns.

3. How is reproducibility handled?
Answer: The engine supports deterministic seeding (`StartNewGame(seed)`) and test state injection.

4. How do you validate a player request?
Answer: Rank parsing + rule validation (`player can only request ranks in hand`).

5. Where are metrics collected?
Answer: In the core engine through `GameMetrics` updates on turns, requests, and outcomes.

## 3) Senior Interview Questions

1. How would you scale this for many concurrent games?
Answer: Externalize state (Redis/PostgreSQL), shard by game/session ID, and deploy stateless API replicas.

2. How would you support multi-environment deployment?
Answer: Environment-specific config via `.env` + secret manager + CI deployment matrices.

3. How would you instrument observability at enterprise level?
Answer: Add OpenTelemetry traces, structured logs, Prometheus counters/histograms, and SLO dashboards.

4. How would you harden security?
Answer: Introduce auth tokens, rate limits, input throttling, and secret scanning in CI.

5. What are the core trade-offs of in-memory engines?
Answer: Excellent speed/simplicity, but weak durability and poor horizontal state sharing.

## 4) Critical Code Sections

### A) Game Rule Engine
File: `src/GoFish.Core/Engine/GoFishGameEngine.cs`
- `PlayerAsk(...)` orchestrates one complete player command lifecycle.
- `ExecuteAsk(...)` implements transfer/go-fish behavior.
- `ResolveBooks(...)` enforces scoring rules.

Interview angle:
- Explain how you guarantee consistency across multiple state mutations in one command.

### B) Input Normalization
File: `src/GoFish.Core/Domain/CardRankParser.cs`
- Converts user tokens (`A`, `10`, `Queen`, etc.) into strict enum values.

Interview angle:
- Discuss defensive parsing and why transport formats should not leak into domain rules.

### C) Controller/Service Layer
File: `src/GoFish.Application/Services/GameControllerService.cs`
- Maps domain snapshot to API DTOs.
- Applies synchronization lock for safe access to shared game state.

Interview angle:
- Justify lock scope and mention how this evolves to distributed locks when state is externalized.

### D) API Edge Layer
File: `src/GoFish.Api/Program.cs`
- Exposes game endpoints and serves static dashboard assets.

Interview angle:
- Discuss why keeping HTTP layer thin protects core logic from framework churn.
