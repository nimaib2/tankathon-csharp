import { Fragment } from "react";
import type { BracketGame, Region, Team } from "../types";
import GameSlot from "./GameSlot";
import styles from "./RegionBracket.module.css";

interface Props {
  region: Region;
  games: BracketGame[];
  teams: Map<string, Team>;
  onPickTeam?: (gameId: string, teamId: string) => void;
}

const ROUND_LABELS: Record<number, string> = {
  1: "R64",
  2: "R32",
  3: "Sweet 16",
  4: "Elite 8",
};

export default function RegionBracket({ region, games, teams, onPickTeam }: Props) {
  const regionGames = games.filter((g) => g.regionId === region.id);
  const rounds = [1, 2, 3, 4].map((r) => ({
    round: r,
    label: ROUND_LABELS[r],
    games: regionGames
      .filter((g) => g.round === r)
      .sort((a, b) => a.gameIndex - b.gameIndex),
  }));

  return (
    <div className={styles.region}>
      <h2 className={styles.title}>{region.name}</h2>
      <div className={styles.bracket}>
        {rounds.map(({ round, label, games: roundGames }, ri) => (
          <Fragment key={round}>
            {ri > 0 && (
              <div className={styles.connectorCol}>
                <div className={styles.connectorLabel} />
                {roundGames.map((_, i) => (
                  <div key={i} className={styles.connector} />
                ))}
              </div>
            )}
            <div className={styles.roundCol}>
              <div className={styles.roundLabel}>{label}</div>
              <div className={styles.gamesCol}>
                {roundGames.map((g) => (
                  <div key={g.id} className={styles.gameWrapper}>
                    {ri > 0 && <div className={styles.lineIn} />}
                    <GameSlot game={g} teams={teams} onPickTeam={onPickTeam} />
                    {ri < rounds.length - 1 && <div className={styles.lineOut} />}
                  </div>
                ))}
              </div>
            </div>
          </Fragment>
        ))}
      </div>
    </div>
  );
}
