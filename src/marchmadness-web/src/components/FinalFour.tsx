import type { BracketGame, Team } from "../types";
import GameSlot from "./GameSlot";
import styles from "./FinalFour.module.css";

interface Props {
  games: BracketGame[];
  teams: Map<string, Team>;
}

export default function FinalFour({ games, teams }: Props) {
  const semis = games.filter((g) => g.round === 5).sort((a, b) => a.gameIndex - b.gameIndex);
  const championship = games.filter((g) => g.round === 6);
  const champion = championship[0]?.winnerId
    ? teams.get(championship[0].winnerId)
    : null;

  return (
    <div className={styles.container}>
      <h2 className={styles.title}>Final Four</h2>
      <div className={styles.grid}>
        <div className={styles.column}>
          <div className={styles.label}>Semifinals</div>
          {semis.map((g) => (
            <GameSlot key={g.id} game={g} teams={teams} />
          ))}
        </div>
        {championship.length > 0 && (
          <div className={styles.column}>
            <div className={styles.label}>Championship</div>
            {championship.map((g) => (
              <GameSlot key={g.id} game={g} teams={teams} />
            ))}
          </div>
        )}
      </div>
      {champion && (
        <div className={styles.champion}>
          <div className={styles.championLabel}>National Champion</div>
          <div className={styles.championName}>{champion.name}</div>
        </div>
      )}
    </div>
  );
}
