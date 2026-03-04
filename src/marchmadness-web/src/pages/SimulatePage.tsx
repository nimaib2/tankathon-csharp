import { useEffect, useState } from "react";
import { fetchBracket, simulateBracket } from "../api";
import type { Bracket, Team } from "../types";
import RegionBracket from "../components/RegionBracket";
import FinalFour from "../components/FinalFour";
import styles from "./SimulatePage.module.css";

export default function SimulatePage() {
  const [bracket, setBracket] = useState<Bracket | null>(null);
  const [simulated, setSimulated] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchBracket()
      .then(setBracket)
      .catch((e) => setError(e.message));
  }, []);

  async function handleSimulate() {
    setLoading(true);
    setError(null);
    try {
      const result = await simulateBracket();
      setBracket(result);
      setSimulated(true);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Simulation failed");
    } finally {
      setLoading(false);
    }
  }

  async function handleReset() {
    setLoading(true);
    try {
      const result = await fetchBracket();
      setBracket(result);
      setSimulated(false);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Reset failed");
    } finally {
      setLoading(false);
    }
  }

  if (error && !bracket)
    return <div className={styles.error}>Error: {error}</div>;
  if (!bracket) return <div className={styles.loading}>Loading bracket...</div>;

  const teamMap = new Map<string, Team>(bracket.teams.map((t) => [t.id, t]));
  const finalFourGames = bracket.games.filter((g) => g.round >= 5);

  return (
    <div>
      <div className={styles.header}>
        <div>
          <h1 className={styles.title}>Simulate Tournament</h1>
          {error && <div className={styles.inlineError}>{error}</div>}
        </div>
        <div className={styles.controls}>
          <button
            className={styles.btnPrimary}
            onClick={handleSimulate}
            disabled={loading || simulated}
          >
            {loading ? "Simulating..." : "Simulate Full Bracket"}
          </button>
          {simulated && (
            <button
              className={styles.btnSecondary}
              onClick={handleReset}
              disabled={loading}
            >
              Reset
            </button>
          )}
        </div>
      </div>
      {bracket.regions.map((region) => (
        <RegionBracket
          key={region.id}
          region={region}
          games={bracket.games}
          teams={teamMap}
        />
      ))}
      {finalFourGames.length > 0 && (
        <FinalFour games={finalFourGames} teams={teamMap} />
      )}
    </div>
  );
}
