# NBA Draft Lottery Website – Project Outline

A Tankathon-style site for NBA standings, draft lottery simulation, and mock drafts. Use this as the blueprint; implement features on top of the structure below.

---

## 1. Tech Stack

| Layer | Recommendation | Alternatives |
|-------|----------------|--------------|
| **Backend** | ASP.NET Core 8+ Web API | — |
| **Frontend** | Blazor Web App (SSR + interactivity) or React/Vue SPA | MVC + Razor, or API-only + any SPA |
| **Data** | Entity Framework Core + SQL Server / SQLite | PostgreSQL, in-memory for MVP |
| **NBA Data** | External NBA API or scraped standings | nba_api-style endpoints, BallDontLie API, or manual CSV/JSON |
| **Hosting** | Azure App Service, Linux container, or any .NET host | — |

Start with **Web API + Blazor** in one solution so you can add pages and call the same API from the UI.

---

## 2. High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│  Frontend (Blazor / SPA)                                         │
│  • Standings page • Lottery simulator • Mock draft builder       │
└──────────────────────────────┬──────────────────────────────────┘
                               │ HTTP/JSON
┌──────────────────────────────▼──────────────────────────────────┐
│  ASP.NET Core Web API                                            │
│  • StandingsController • LotteryController • MockDraftController │
└──────────────────────────────┬──────────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│  Application / Domain                                            │
│  • StandingsService • LotteryService • MockDraftService          │
│  • Lottery odds engine • Draft order logic                       │
└──────────────────────────────┬──────────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│  Data & External                                                 │
│  • DbContext (teams, standings, prospects, draft results)        │
│  • INbaStandingsProvider (external API or file-based)            │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Core Features (MVP → Later)

### Phase 1 – Foundation
- **Standings**
  - Show current (or last completed) NBA standings for lottery-eligible teams (non-playoff).
  - Columns: rank, team, W, L, win %, lottery odds (top-4 and #1).
- **Standings data source**
  - Sync from external source (API or CSV/JSON) into your DB, or serve from provider with caching.
- **Lottery odds**
  - Store or compute odds per rank (1–14) using the current NBA table (post-2019 flattened odds).

### Phase 2 – Lottery Simulator
- **Simulate lottery**
  - Input: current standings (or selected season).
  - Output: random draft order for picks 1–14 using official-style odds (e.g., weighted draw, then remaining order by record).
- **Multiple runs**
  - “Run 1 / 10 / 100” simulations and show aggregate (e.g., how often each team got #1).

### Phase 3 – Mock Draft
- **Prospect list**
  - Manage a list of prospects (name, position, school, rank, year).
  - Seed from CSV/API or manual entry.
- **Mock draft**
  - Given a draft order (from standings or from a lottery result), auto-assign prospects to teams (e.g., best available or by simple “team need” rules).
  - Allow user to override picks (swap, change selection).
- **Save / share**
  - Optional: save mock draft to DB and share via link.

### Phase 4+ (Later)
- Draft big board, power rankings for picks, remaining schedule strength, historical drafts, user accounts, and multiple leagues (e.g., WNBA) if desired.

---

## 4. Data Model (Core Entities)

- **Team**  
  Id, Name, Abbreviation, Conference, Division, LogoUrl (optional).

- **StandingsSnapshot** (or SeasonStandings)  
  Id, Season (e.g. "2024-25"), TeamId, Wins, Losses, WinPercentage, Rank, CreatedAt.  
  Optional: GamesBack, Conference, DivisionRank.

- **LotteryOdds**  
  Rank (1–14), OddsForFirstPick, OddsForTopFour, etc. (or a single JSON column for full odds table).  
  Can be config/table; one row per rank.

- **Prospect**  
  Id, Name, Position, School, ClassYear, OverallRank, DraftYear, Source (e.g. “Tankathon”, “Manual”).

- **DraftLotteryResult** (simulation run)  
  Id, Season, RunAt, ResultOrder (JSON: [ { Pick: 1, TeamId }, … ]), Seed (for reproducibility, optional).

- **MockDraft** (optional for Phase 3)  
  Id, Title, Season, DraftOrder (JSON or linked to DraftLotteryResult), Picks (JSON or related MockDraftPick table), CreatedAt.

Keep relationships simple: Team ↔ StandingsSnapshot, Team ↔ DraftLotteryResult picks, Prospect ↔ MockDraft picks.

---

## 5. NBA Draft Lottery – Simulation Logic

- **Eligible teams**  
  Top 14 teams by inverse standings (worst 14, or “non-playoff” if you prefer).  
  Order by worst WinPercentage (or fewest wins); tie-break per NBA rules if you want (e.g., random draw).

- **Odds**  
  Use the official post-2019 table (14% / 14% / 14% for worst 3, then descending). Store in `LotteryOdds` or config.

- **Draw**  
  - Assign each team a set of 4-digit combinations (1–14, no repeat); number of combinations per team = proportional to odds.  
  - Draw 4 balls (without replacement) to get one combination → that team gets pick 1.  
  - Remove that team; redraw for pick 2, then 3, then 4.  
  - Picks 5–14: fill in remaining teams in reverse order of standings (worst remaining gets 5, etc.).

- **Implementation**  
  - Either precompute all combinations and map to teams, or use weighted random (by odds) for picks 1–4, then assign 5–14 by order.  
  - Expose a `ILotteryEngine` with something like `Draw(StandingsSnapshot) → DraftOrder`.

---

## 6. External Data – NBA Standings

- **Option A – Free public endpoints**  
  Use stats.nba.com–style endpoints (similar to what nba_api uses). You’ll need a small client (HttpClient) and possibly cookie/header handling; no key required but may be brittle.

- **Option B – BallDontLie / other free APIs**  
  If they provide standings or games, derive standings from game results or use their standings endpoint if available.

- **Option C – Manual / file**  
  CSV or JSON upload or pasted data for “current” standings; good for testing and fallback.

- **Recommendation**  
  Define `INbaStandingsProvider` that returns `List<TeamStanding>`. Implement one adapter (e.g., API or CSV); swap later without changing the rest of the app.

---

## 7. Suggested Solution Layout

```
Tankathon.sln
├── src/
│   ├── Tankathon.Api/                    # ASP.NET Core Web API
│   │   ├── Controllers/
│   │   │   ├── StandingsController.cs
│   │   │   ├── LotteryController.cs
│   │   │   └── MockDraftController.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── Tankathon.Core/                   # Domain & interfaces
│   │   ├── Entities/
│   │   ├── Interfaces/
│   │   │   ├── IStandingsService.cs
│   │   │   ├── ILotteryService.cs
│   │   │   ├── IMockDraftService.cs
│   │   │   └── INbaStandingsProvider.cs
│   │   └── Models/                       # DTOs, request/response
│   ├── Tankathon.Application/            # Application logic
│   │   ├── Services/
│   │   │   ├── StandingsService.cs
│   │   │   ├── LotteryService.cs
│   │   │   └── MockDraftService.cs
│   │   └── Lottery/                      # Lottery engine
│   │       └── LotteryEngine.cs
│   └── Tankathon.Infrastructure/         # Data & external
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   └── Migrations/
│       ├── Repositories/                 # if not using EF directly in services
│       └── NbaStandings/                 # INbaStandingsProvider impl
│           └── NbaApiStandingsProvider.cs  (or CsvStandingsProvider)
├── tests/
│   └── Tankathon.Tests/
│       └── LotteryEngineTests.cs
└── docs/
    └── PROJECT_OUTLINE.md                # this file
```

You can add a **Tankathon.Web** (Blazor) project under `src/` later and have it call the API or reference Application services directly if you run in one process.

---

## 8. API Endpoints (Sketch)

| Method | Path | Purpose |
|--------|------|--------|
| GET | `/api/standings?season=2024-25` | Current lottery standings + odds |
| GET | `/api/standings/refresh` or POST | Trigger sync from external source (if you cache) |
| POST | `/api/lottery/simulate` | Body: optional standings override; response: draft order 1–14 |
| POST | `/api/lottery/simulate-many?runs=100` | Multiple simulations, return counts per team for #1 / top-4 |
| GET | `/api/prospects?draftYear=2025` | List prospects for mock draft |
| POST | `/api/mock-draft` | Create mock draft (order + optional picks) |
| GET | `/api/mock-draft/{id}` | Get a saved mock draft |
| PUT | `/api/mock-draft/{id}/picks` | Update picks (e.g., swap) |

---

## 9. Lottery Odds Reference (Post-2019)

Use a table like this (simplified; you can store full top-4 breakdown):

| Rank (worst=1) | Odds #1 (approx) | Odds Top 4 |
|----------------|-------------------|------------|
| 1 | 14.0% | 52.1% |
| 2 | 14.0% | 52.1% |
| 3 | 14.0% | 52.1% |
| 4 | 12.5% | 48.2% |
| 5 | 10.5% | 42.1% |
| 6 | 9.0% | 37.2% |
| 7 | 7.5% | 31.9% |
| 8 | 6.0% | 26.3% |
| 9 | 4.5% | 19.9% |
| 10 | 3.0% | 13.4% |
| 11 | 2.0% | 9.0% |
| 12 | 1.5% | 6.7% |
| 13 | 1.0% | 4.5% |
| 14 | 0.5% | 2.3% |

Source: NBA; double-check against official NBA.com for the exact decimal table.

---

## 10. Implementation Order

1. **Solution + projects** – Api, Core, Application, Infrastructure; DI wiring.
2. **Db + entities** – Team, StandingsSnapshot, LotteryOdds; EF migrations.
3. **Standings** – INbaStandingsProvider (stub or CSV), StandingsService, GET standings API, optional refresh.
4. **Lottery** – LotteryEngine (weighted draw), LotteryService, simulate + simulate-many endpoints.
5. **Frontend** – Standings page, “Simulate lottery” button, results table.
6. **Prospects** – Entity + seed data, GET prospects API.
7. **Mock draft** – Create/update mock draft, assign prospects by order; mock draft API and UI.

After that, add polish: big board, power rankings, schedule strength, auth, and multiple seasons.

---

## 11. Notes

- **Caching:** Cache standings in DB or in-memory (e.g., 15–60 min) to avoid hammering the NBA data source.
- **CORS:** If the frontend is a separate SPA, enable CORS in the API for that origin.
- **Secrets:** Put any API keys (if you add a paid provider) in User Secrets or Azure Key Vault, not in repo.
- **Tests:** Unit-test the lottery engine (fixed seed) so draw probabilities match the odds table.

This outline gives you a clear path from “standings + lottery sim” to “mock drafts” while staying in C# and .NET. You can implement each section on top of this structure as you go.
