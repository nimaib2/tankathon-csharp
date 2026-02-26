# Tankathon (C# / .NET)

A Tankathon-style site for **NBA standings**, **draft lottery simulation**, and **mock drafts** built with C# and ASP.NET Core.

## Quick start

- **Outline & architecture:** See [PROJECT_OUTLINE.md](PROJECT_OUTLINE.md) for the full plan (features, data model, API sketch, lottery logic, implementation order).
- **Run the API** (terminal 1):
  ```bash
  dotnet run --project src/Tankathon.Api
  ```
  Listens on `http://localhost:5111`.
- **Run the Blazor frontend** (terminal 2):
  ```bash
  dotnet run --project src/Tankathon.Web
  ```
  Open `http://localhost:5181` in your browser. Use **Standings** and **Lottery Simulator** from the nav (the Web app calls the API at 5111).
- **API endpoints** (for direct calls):
  - `GET http://localhost:5111/api/standings?season=2024-25` — lottery standings (stub data)
  - `POST http://localhost:5111/api/lottery/simulate` — body `{}` or `{"season":"2024-25","seed":42}` — one lottery run
  - `POST http://localhost:5111/api/lottery/simulate-many?runs=10` — multiple simulations

## Solution layout

| Project | Role |
|--------|------|
| **Tankathon.Api** | Web API (standings, lottery, mock draft controllers) |
| **Tankathon.Web** | Blazor Web App frontend (Standings, Lottery Simulator pages) |
| **Tankathon.Core** | Entities, DTOs, interfaces (`IStandingsService`, `ILotteryService`, `INbaStandingsProvider`) |
| **Tankathon.Application** | Services + `LotteryEngine` (weighted draw for picks 1–14) |
| **Tankathon.Infrastructure** | Standings provider (currently `StubStandingsProvider`; replace with NBA API or CSV) |

Implement features on top of this structure using the phases in [PROJECT_OUTLINE.md](PROJECT_OUTLINE.md).