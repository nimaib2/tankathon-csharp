import type { BracketGame, Team } from "../types";
import styles from "./GameSlot.module.css";

interface Props {
  game: BracketGame;
  teams: Map<string, Team>;
  onPickTeam?: (gameId: string, teamId: string) => void;
}

export default function GameSlot({ game, teams, onPickTeam }: Props) {
  const home = game.homeTeamId ? teams.get(game.homeTeamId) : null;
  const away = game.awayTeamId ? teams.get(game.awayTeamId) : null;
  const winnerId = game.winnerId;
  const interactive = !!onPickTeam;

  return (
    <div className={`${styles.slot} ${winnerId ? styles.played : ""}`}>
      <TeamRow
        team={home}
        score={game.scoreHome}
        isWinner={winnerId === home?.id}
        interactive={interactive && !!home}
        onClick={() => home && onPickTeam?.(game.id, home.id)}
      />
      <div className={styles.divider} />
      <TeamRow
        team={away}
        score={game.scoreAway}
        isWinner={winnerId === away?.id}
        interactive={interactive && !!away}
        onClick={() => away && onPickTeam?.(game.id, away.id)}
      />
    </div>
  );
}

function TeamRow({
  team,
  score,
  isWinner,
  interactive,
  onClick,
}: {
  team: Team | null | undefined;
  score: number | null;
  isWinner: boolean;
  interactive?: boolean;
  onClick?: () => void;
}) {
  const cls = [
    styles.team,
    isWinner ? styles.winner : "",
    interactive ? styles.clickable : "",
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <div className={cls} onClick={interactive ? onClick : undefined}>
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
