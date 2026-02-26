# March Madness Bracket Simulator – Project Outline

A C# / .NET bracket simulator aligned with **2025–26 NCAA Men's Basketball** and ESPN-style bracketology. Use this as the base to add simulation, picks, and UI features.

---

## 1. Tournament structure (2025–26)

- **Field:** 68 teams (31 auto bids + 37 at-large).
- **First Four:** 4 play-in games (8 teams); winners fill the four 11/16 slots in the main bracket.
- **Main bracket:** 64 teams in **4 regions**, each with seeds **1–16**.
- **Rounds:** First Four → Round of 64 → 32 → 16 (Sweet 16) → 8 (Elite Eight) → Final Four → National Championship (7 rounds total for the 68-team event).
- **ESPN 2025–26 reference:** Top overall seed (e.g. Purdue), Last In / First Out for bubble, conference bid counts (e.g. Big Ten 11). Design your seed list and regions so you can plug in ESPN’s bracketology when you have it.

---

## 2. Solution structure (suggested)

```
MarchMadness.sln
├── src/
│   ├── MarchMadness.Api/           # ASP.NET Core Web API (optional; can start with Blazor only)
│   ├── MarchMadness.Web/           # Blazor Web App (SSR + interactivity)
│   ├── MarchMadness.Application/   # Bracket logic, simulation, application services
│   ├── MarchMadness.Core/          # Domain models, interfaces
│   └── MarchMadness.Infrastructure/# Seed data, external/ESPN-aligned data (stub or API)
```

- **Core:** No dependencies. Entities and interfaces only.
- **Application:** Depends on Core. Implements bracket building, game simulation, scoring.
- **Infrastructure:** Depends on Core (and optionally Application). Implements `IBracketSeedProvider`, file/API data.
- **Api (optional):** Depends on Application + Infrastructure. REST endpoints for bracket, games, results.
- **Web:** Depends on Application (and optionally Api). Blazor UI for bracket view, picks, simulation.

Start with **Core → Application → Web**; add Api and Infrastructure when you need external data or a separate API.

---

## 3. Core – domain and contracts

### 3.1 Models (`MarchMadness.Core/Models/`)

- **`Team`**  
  - `Id`, `Name`, `ShortName`, `Conference`, `Seed` (1–16), `RegionId`, `IsFirstFour` (for play-in teams).

- **`Region`**  
  - `Id`, `Name` (e.g. East, South, Midwest, West).

- **`BracketSlot`**  
  - Represents one “line” in the bracket: `SlotId` (e.g. round + game index), `Round` (1–6 for 64-team bracket; 0 for First Four if you model it), `RegionId`, `Seed1`, `Seed2`, `WinnerTeamId` (nullable), `GameIndex`.

- **`Game` or `BracketGame`**  
  - `Id`, `Round`, `RegionId`, `HomeTeamId`, `AwayTeamId`, `WinnerId`, `ScoreHome`, `ScoreAway`, `PlayedAt` (optional).  
  - For First Four, round can be 0 or a dedicated enum.

- **`Bracket`**  
  - `Id`, `Season` (e.g. "2025-26"), `Regions`, `Teams`, `Games`, `Source` (e.g. "ESPN Bracketology Jan 2026").  
  - Holds the full 68-team field and all games/slots.

- **`Conference`** (optional)  
  - `Id`, `Name` – for standings and “conference bid” display consistent with ESPN.

### 3.2 Interfaces (`MarchMadness.Core/Interfaces/`)

- **`IBracketSeedProvider`**  
  - `Task<Bracket> GetBracketAsync(string season, CancellationToken ct = default);`  
  - Returns 68 teams placed in regions and First Four; aligns with your chosen source (e.g. ESPN 2025–26).

- **`IBracketSimulationEngine`**  
  - `Task<BracketResult> SimulateGameAsync(Game game, CancellationToken ct = default);`  
  - `Task<Bracket> SimulateFullBracketAsync(Bracket bracket, CancellationToken ct = default);`  
  - You can extend with different strategies (e.g. seed-based odds, KenPom-style, random).

- **`IStandingsProvider`** (optional)  
  - For “standings consistent with ESPN”: `Task<IReadOnlyList<ConferenceStanding>> GetConferenceStandingsAsync(string season, CancellationToken ct);`  
  - Use later to show conference tables or validate auto-bid logic.

---

## 4. Application – bracket and simulation

### 4.1 Services (`MarchMadness.Application/Services/`)

- **`BracketService`**  
  - Uses `IBracketSeedProvider` to load the bracket.  
  - Builds `Bracket` with all `BracketSlot`/`Game` entries for Round of 64 → Championship.  
  - Handles First Four: 4 games → 4 winners into the main 64-team grid.

- **`BracketSimulationService`**  
  - Uses `IBracketSimulationEngine` to run a single game or full bracket.  
  - Updates `Game.WinnerId` and propagates winners into the next round.  
  - Returns the completed bracket (or round-by-round if you want to animate).

### 4.2 Simulation engine (simple first version)

- **`SeedBasedSimulationEngine`** (in Application or Infrastructure)  
  - Implements `IBracketSimulationEngine`.  
  - Win probability by seed (e.g. 1 vs 16 → ~99% for 1; 8 vs 9 → ~50%).  
  - Use a shared lookup table or formula so it’s easy to swap with “ESPN strength” or KenPom later.

### 4.3 DTOs (optional)

- **`BracketDto`** – regions, teams, games for API/Blazor.  
- **`GameResultDto`** – `GameId`, `WinnerId`, `ScoreHome`, `ScoreAway`.  
- **`SimulationOptionsDto`** – e.g. random seed, engine type, number of runs (for Monte Carlo later).

---

## 5. Infrastructure – 2025–26 data

- **`ESPNAlignedSeedProvider`** or **`StaticBracketSeedProvider`**  
  - Implements `IBracketSeedProvider`.  
  - **Static:** JSON file with 68 teams, regions, seeds, First Four matchups. You maintain this from ESPN’s 2025–26 bracketology (regions, seeds, Last In / First Out).  
  - **ESPN-aligned:** Later you can scrape or use an API if available; keep the same `Bracket` model so the rest of the app is unchanged.

- **Seed data shape (e.g. `seed-data-2025-26.json`)**  
  - List of teams: `TeamId`, `Name`, `Conference`, `Seed`, `Region`, `IsFirstFour`, `FirstFourSlot` (e.g. "11a", "11b").  
  - Optional: `DisplayOrder`, `BidType` (Auto/AtLarge) for UI.

- **Regions**  
  - East, South, Midwest, West (or current NCAA names for 2025–26).  
  - Top overall seed in one region; 2–4 in others (you can encode S-curve in Application when building the bracket).

---

## 6. API (optional)

If you add **MarchMadness.Api**:

- **GET** `/api/bracket?season=2025-26` – full bracket (teams, regions, games, no results).  
- **GET** `/api/bracket/games?season=2025-26&round=1` – games for a round.  
- **POST** `/api/bracket/simulate` – body: `{ "season": "2025-26", "fullBracket": true }` – returns bracket with simulated results.  
- **GET** `/api/standings?season=2025-26` – conference standings (if you implement `IStandingsProvider`).

Use same DTOs as Blazor so you can call the API from the client or run everything in-process in Blazor.

---

## 7. Web (Blazor)

- **Pages/routes**  
  - `/` – home (links to bracket, simulate, about).  
  - `/bracket` – view current 2025–26 bracket (teams by region, First Four, empty results).  
  - `/bracket/simulate` – “Run simulation” → one-shot or step-through; show winner per game and champion.  
  - `/standings` (optional) – conference standings table (ESPN-consistent).

- **Components**  
  - **`BracketView`** – four regions, each with 1–16 seeds and matchups; highlight First Four slots.  
  - **`RegionBracket`** – one region’s tree (rounds 1–4).  
  - **`GameSlot`** – one matchup (team names, seed, optional score/winner).  
  - **`SimulationControls`** – “Simulate round”, “Simulate all”, “Reset”.

- **State**  
  - Hold current `Bracket` (and optional list of `Game` results) in a service or Blazor state; bracket load from `IBracketSeedProvider` (server-side) or from API.

- **Styling**  
  - CSS (or Tailwind) for bracket lines and responsive layout; keep naming/seed order consistent with ESPN (e.g. 1–16 down the region).

---

## 8. Consistency with 2025–26 NCAA and ESPN

- **Standings:** When you add standings, use conference names and (if possible) win/loss records that match ESPN’s 2025–26 season; update `IStandingsProvider` and your data source.  
- **Bracketology:** Populate `StaticBracketSeedProvider` from ESPN’s 2026 bracketology (regions, seeds, Last Four In / First Four Out). Update the JSON when ESPN updates (e.g. Tuesdays/Fridays).  
- **Seeding rules:** In Application, when building the bracket from a seed list, apply NCAA S-curve (1–4 in different regions, 5–8, etc.) so your placement matches how ESPN/NCAA show the bracket.

---

## 9. Features to add on top

- **User brackets:** Save user picks (game-by-game or full bracket); compare to simulated or real results.  
- **Scoring:** Standard bracket scoring (1–2–4–8–16–32) or custom.  
- **Monte Carlo:** Run N simulations, show win probability per team and “most likely champion.”  
- **Historical data:** Past seasons’ brackets and results for “replay” or validation.  
- **Live data:** Replace static seed data with an API when available.  
- **Mobile-friendly bracket:** Touch-friendly region/game navigation and collapse/expand.

---

## 10. Suggested implementation order

1. **Core:** `Team`, `Region`, `Bracket`, `Game`, `IBracketSeedProvider`, `IBracketSimulationEngine`.  
2. **Infrastructure:** `StaticBracketSeedProvider` + one `seed-data-2025-26.json` (minimal 68 teams, 4 regions, First Four).  
3. **Application:** `BracketService` (build bracket from provider), `BracketSimulationService`, `SeedBasedSimulationEngine`.  
4. **Web:** Blazor app, `BracketView` + `RegionBracket` + `GameSlot`, load bracket and “Simulate all” button.  
5. **Optional:** Api project, standings provider, then user brackets and scoring.

This gives you a single solution with 2025–26 structure and ESPN-aligned data entry points, and a clear place for each new feature.
