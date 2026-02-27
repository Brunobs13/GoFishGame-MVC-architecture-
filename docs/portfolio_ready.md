# Portfolio Ready Pack

## LinkedIn Version (Short)
I rebuilt an academic Go Fish project into a production-structured platform using .NET 8 and MVC layering. The solution includes a clean domain engine, REST API, automated tests, CI pipeline, Docker deployment, and an artistic real-time dashboard. Focus: architecture quality, maintainability, and interview-ready engineering standards.

## CV Version (Technical)
Engineered a full repository modernization for a legacy C#/MonoGame project:
- Designed clean multi-project architecture (`Core`, `Application`, `API`, `Tests`, `Web`)
- Implemented deterministic game engine with metrics/event stream
- Delivered REST endpoints, responsive dashboard, and Docker runtime
- Added CI/CD workflow, security-oriented `.gitignore`, and technical documentation suite
- Refactored legacy structure and removed tracked generated artifacts

## 60-Second Pitch
I took a classroom Go Fish implementation and rebuilt it as if it were a company backend product. I separated domain rules into a core engine, added an application controller layer, exposed everything through a REST API, and built a responsive web dashboard for gameplay and metrics. I also added tests, CI, Docker, and full technical docs. The result is a maintainable, deployable, interview-ready project that demonstrates architecture, code quality, and DevOps hygiene.

## 5-Minute Technical Pitch
This project started as an academic codebase with mixed responsibilities and tracked build artifacts. I restructured it into a clean architecture: `GoFish.Core` contains all rules and state transitions, `GoFish.Application` handles orchestration and DTO mapping, and `GoFish.Api` exposes a stable HTTP interface while serving a static dashboard from `web/`.

The game engine supports deterministic seeds for reproducibility, validates requests by rule constraints, and tracks operational metrics such as total requests, successful requests, go-fish events, and turns processed. I added a rolling event stream to improve runtime transparency.

On the delivery side, I implemented xUnit tests for critical gameplay rules, a GitHub Actions pipeline for restore/build/test gates, and Docker artifacts for portable execution. I also cleaned repository hygiene by removing generated binaries and enforcing a robust `.gitignore` that includes both .NET and MLOps-style exclusions (`mlruns`, `artifacts`, `.dvc/cache`, `.env`).

If scaling was required, the next step would be externalizing in-memory state to Redis/PostgreSQL, introducing session IDs, and adding OpenTelemetry + Prometheus for full observability.
