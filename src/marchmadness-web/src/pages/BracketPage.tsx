import { useEffect, useState } from "react";
import { fetchBracket } from "../api";
import type { Bracket, Team } from "../types";
import { useBracketPicks } from "../hooks/useBracketPicks";
import RegionBracket from "../components/RegionBracket";
import FinalFour from "../components/FinalFour";
import styles from "./BracketPage.module.css";

export default function BracketPage() {
  const [bracket, setBracket] = useState<Bracket | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchBracket()
      .then(setBracket)
      .catch((e) => setError(e.message));
  }, []);

  if (error) return <div className={styles.error}>Error: {error}</div>;
  if (!bracket)
    return <div className={styles.loading}>Loading bracket...</div>;

  return <BracketWithPicks bracket={bracket} />;
}

function BracketWithPicks({ bracket }: { bracket: Bracket }) {
  const { games, pickWinner, resetPicks, hasPicks } = useBracketPicks(
    bracket.games
  );
  const teamMap = new Map<string, Team>(bracket.teams.map((t) => [t.id, t]));
  const finalFourGames = games.filter((g) => g.round >= 5);

  return (
    <div>
      <div className={styles.header}>
        <div>
          <h1 className={styles.title}>2025–26 Bracket</h1>
          {bracket.source && (
            <span className={styles.source}>{bracket.source}</span>
          )}
          <p className={styles.hint}>Click a team to advance them</p>
        </div>
        {hasPicks && (
          <button className={styles.btnReset} onClick={resetPicks}>
            Reset Picks
          </button>
        )}
      </div>
      {bracket.regions.map((region) => (
        <RegionBracket
          key={region.id}
          region={region}
          games={games}
          teams={teamMap}
          onPickTeam={pickWinner}
        />
      ))}
      {finalFourGames.length > 0 && (
        <FinalFour
          games={finalFourGames}
          teams={teamMap}
          onPickTeam={pickWinner}
        />
      )}
    </div>
  );
}
