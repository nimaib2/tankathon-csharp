import type { Bracket } from "./types";

const API_BASE = "http://127.0.0.1:5050";

export async function fetchBracket(season = "2025-26"): Promise<Bracket> {
  const res = await fetch(`${API_BASE}/api/bracket?season=${season}`);
  if (!res.ok) throw new Error(`Failed to fetch bracket: ${res.status}`);
  return res.json();
}

export async function simulateBracket(season = "2025-26"): Promise<Bracket> {
  const res = await fetch(`${API_BASE}/api/bracket/simulate?season=${season}`, {
    method: "POST",
  });
  if (!res.ok) throw new Error(`Failed to simulate bracket: ${res.status}`);
  return res.json();
}
