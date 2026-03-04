import type { BracketGame, Region, Team } from "../types";
import GameSlot from "./GameSlot";
import styles from "./RegionBracket.module.css";

interface Props {
  region: Region;
  games: BracketGame[];
  teams: Map<string, Team>;
}

const ROUND_LABELS: Record<number, string> = {
  1: "Round of 64",
  2: "Round of 32",
  3: "Sweet 16",
  4: "Elite Eight",
};

export default function RegionBracket({ region, games, teams }: Props) {
  const regionGames = games.filter((g) => g.regionId === region.id);
  const rounds = [1, 2, 3, 4].map((r) => ({
    round: r,
    label: ROUND_LABELS[r],
    games: regionGames.filter((g) => g.round === r).sort((a, b) => a.gameIndex - b.gameIndex),
  }));

  return (
    <div className={styles.region}>
      <h2 className={styles.title}>{region.name}</h2>
      <div className={styles.rounds}>
        {rounds.map(({ round, label, games: roundGames }) => (
          <div key={round} className={styles.round}>
            <div className={styles.roundLabel}>{label}</div>
            <div
              className={styles.gamesColumn}
              style={{ gap: `${Math.pow(2, round - 1) * 0.5}rem` }}
            >
              {roundGames.map((g) => (
                <GameSlot key={g.id} game={g} teams={teams} />
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
