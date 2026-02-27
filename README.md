# GoFish Control Deck (MVC Architecture)

A production-structured evolution of the original academic Go Fish implementation.

This repository now delivers a clean MVC-oriented backend architecture, REST API, automated tests, CI pipeline, containerization, and an artistic web dashboard for interactive gameplay.

## Project Overview
GoFish Control Deck is a lightweight game-processing platform that demonstrates how to transform a classroom codebase into a portfolio-ready engineering project. The core game rules are encapsulated in a reusable domain engine, exposed through an HTTP API, and visualized through a responsive web interface.

## Business Problem
Academic projects usually fail interview standards because they lack structure, observability, and deployment readiness. This project solves that by showing:
- Clean architecture separation (Core, Application, API, UI)
- Reproducible execution and testing
- Production hygiene (CI, Docker, .gitignore, security practices)
- Clear technical storytelling for interviews

## Architecture Diagram (Text)
```
Web Dashboard (HTML/CSS/JS)
        |
        v
ASP.NET Minimal API (GoFish.Api)
        |
        v
Application Controller Service (GoFish.Application)
        |
        v
Game Engine + Domain Rules (GoFish.Core)
        |
        v
In-memory State + Metrics + Event Log
```

MVC interpretation in this codebase:
- Model: `GoFish.Core` domain + engine (`GoFishGameEngine`)
- Controller: `GoFish.Application` orchestration (`GameControllerService`)
- View: `web/` dashboard + API JSON responses

## Tech Stack
- .NET 8
- ASP.NET Core Minimal API
- xUnit for tests
- GitHub Actions for CI
- Docker + Docker Compose
- Vanilla HTML/CSS/JavaScript frontend

## Project Structure
```
.
├── src/
│   ├── GoFish.Core/            # Domain model + game engine
│   ├── GoFish.Application/     # Controller/orchestration + DTO contracts
│   └── GoFish.Api/             # REST API + static web hosting
├── tests/
│   └── GoFish.Core.Tests/      # Unit tests for engine behavior
├── web/                        # Dashboard UI
├── docs/                       # Audit, technical deep dive, portfolio material
├── scripts/                    # Setup/run/test helpers
├── legacy/                     # Original academic implementations
├── Dockerfile
├── docker-compose.yml
├── Makefile
└── GoFish.sln
```

## Setup Instructions
1. Clone the repository:
```bash
git clone https://github.com/Brunobs13/GoFishGame-MVC-architecture-.git
cd GoFishGame-MVC-architecture-
```

2. Restore dependencies:
```bash
./scripts/setup.sh
```

3. Run the API + dashboard locally:
```bash
./scripts/run_api.sh
```

4. Open:
- API Swagger: `http://localhost:5021/swagger` (port may vary)
- Dashboard: `http://localhost:5021/`

5. Run tests:
```bash
./scripts/test.sh
```

## API Endpoints
- `GET /health`
- `GET /api/ranks`
- `GET /api/game/state`
- `GET /api/game/metrics`
- `POST /api/game/reset`
- `POST /api/game/player/ask`

Example action payload:
```json
{
  "rank": "A"
}
```

## CI/CD Overview
GitHub Actions pipeline (`.github/workflows/ci.yml`) runs on push and PR:
1. Restore
2. Build (Release)
3. Test

This enforces baseline quality gates before merge.

## Data Versioning Strategy
This project is state-driven and in-memory, so no dataset artifacts are versioned by default. If extended for analytics, recommended strategy:
- Track datasets with DVC
- Keep generated artifacts out of Git
- Store data lineage metadata in `docs/` and pipeline manifests

## Model Tracking Strategy
No ML model is trained in this project. If a predictive AI dealer is introduced:
- Track experiments with MLflow
- Version feature data with DVC
- Promote model versions via CI gates

## Deployment Strategy
### Docker
```bash
docker compose up --build
```
Application is exposed at `http://localhost:8080`.

### Direct Runtime
Use `dotnet run` through `scripts/run_api.sh`.

## Security Considerations
- No secrets are hardcoded.
- Environment configuration uses `.env` conventions (`.env.example` provided).
- Legacy generated binaries and macOS metadata were removed from version control.
- `.gitignore` covers common leakage vectors (`.env`, logs, build outputs, MLOps artifacts).

## Lessons Learned
- Separating domain rules from transport/UI dramatically improves maintainability.
- Legacy cleanup (tracked binaries, temporary files) is essential for professional perception.
- Deterministic tests require explicit state injection hooks.

## Future Improvements
- Persistent game sessions with Redis/PostgreSQL
- Multi-player rooms via WebSockets
- OpenTelemetry tracing + Prometheus metrics export
- AuthN/AuthZ for shared hosted deployment
- Strategy AI for difficulty levels
