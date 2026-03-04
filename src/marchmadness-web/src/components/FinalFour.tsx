import type { BracketGame, Team } from "../types";
import GameSlot from "./GameSlot";
import styles from "./FinalFour.module.css";

interface Props {
  games: BracketGame[];
  teams: Map<string, Team>;
  onPickTeam?: (gameId: string, teamId: string) => void;
}

export default function FinalFour({ games, teams, onPickTeam }: Props) {
  const semis = games
    .filter((g) => g.round === 5)
    .sort((a, b) => a.gameIndex - b.gameIndex);
  const championship = games.filter((g) => g.round === 6);
  const champion = championship[0]?.winnerId
    ? teams.get(championship[0].winnerId)
    : null;

  return (
    <div className={styles.container}>
      <h2 className={styles.title}>Final Four</h2>
      <div className={styles.bracket}>
        <div className={styles.roundCol}>
          <div className={styles.roundLabel}>Semifinals</div>
          <div className={styles.gamesCol}>
            {semis.map((g) => (
              <div key={g.id} className={styles.gameWrapper}>
                <GameSlot game={g} teams={teams} onPickTeam={onPickTeam} />
                <div className={styles.lineOut} />
              </div>
            ))}
          </div>
        </div>
        <div className={styles.connectorCol}>
          <div className={styles.connectorLabel} />
          <div className={styles.connector} />
        </div>
        <div className={styles.roundCol}>
          <div className={styles.roundLabel}>Championship</div>
          <div className={styles.gamesCol}>
            {championship.map((g) => (
              <div key={g.id} className={styles.gameWrapper}>
                <div className={styles.lineIn} />
                <GameSlot game={g} teams={teams} onPickTeam={onPickTeam} />
              </div>
            ))}
          </div>
        </div>
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
