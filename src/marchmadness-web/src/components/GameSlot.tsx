import type { BracketGame, Team } from "../types";
import styles from "./GameSlot.module.css";

interface Props {
  game: BracketGame;
  teams: Map<string, Team>;
}

export default function GameSlot({ game, teams }: Props) {
  const home = game.homeTeamId ? teams.get(game.homeTeamId) : null;
  const away = game.awayTeamId ? teams.get(game.awayTeamId) : null;
  const winnerId = game.winnerId;

  return (
    <div className={`${styles.slot} ${winnerId ? styles.played : ""}`}>
      <TeamRow
        team={home}
        score={game.scoreHome}
        isWinner={winnerId === home?.id}
      />
      <div className={styles.divider} />
      <TeamRow
        team={away}
        score={game.scoreAway}
        isWinner={winnerId === away?.id}
      />
    </div>
  );
}

function TeamRow({
  team,
  score,
  isWinner,
}: {
  team: Team | null | undefined;
  score: number | null;
  isWinner: boolean;
}) {
  return (
    <div className={`${styles.team} ${isWinner ? styles.winner : ""}`}>
      {team ? (
        <>
          <span className={styles.seed}>{team.seed}</span>
          <span className={styles.name}>{team.shortName}</span>
        </>
      ) : (
        <span className={styles.tbd}>TBD</span>
      )}
      {score != null && <span className={styles.score}>{score}</span>}
    </div>
  );
}
