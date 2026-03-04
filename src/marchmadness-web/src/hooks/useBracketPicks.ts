import { useState, useCallback, useMemo } from "react";
import type { BracketGame } from "../types";

/**
 * After any winner change, do a forward pass through all games (by round)
 * to re-derive each game's homeTeamId/awayTeamId from its source games,
 * then clear any winnerId that's no longer one of the two participants.
 */
function reconcile(games: BracketGame[]): BracketGame[] {
  const map = new Map(games.map((g) => [g.id, g]));
  const sorted = [...games].sort(
    (a, b) => a.round - b.round || a.gameIndex - b.gameIndex
  );

  for (const game of sorted) {
    if (game.sourceGame1Id) {
      game.homeTeamId = map.get(game.sourceGame1Id)?.winnerId ?? null;
    }
    if (game.sourceGame2Id) {
      game.awayTeamId = map.get(game.sourceGame2Id)?.winnerId ?? null;
    }
    if (
      game.winnerId &&
      game.winnerId !== game.homeTeamId &&
      game.winnerId !== game.awayTeamId
    ) {
      game.winnerId = null;
    }
  }
  return sorted;
}

export function useBracketPicks(initialGames: BracketGame[]) {
  const [games, setGames] = useState<BracketGame[]>(() =>
    initialGames.map((g) => ({ ...g }))
  );

  const pickWinner = useCallback((gameId: string, teamId: string) => {
    setGames((prev) => {
      const cloned = prev.map((g) => ({ ...g }));
      const game = cloned.find((g) => g.id === gameId);
      if (!game) return prev;
      // Toggle off if same team clicked again
      game.winnerId = game.winnerId === teamId ? null : teamId;
      return reconcile(cloned);
    });
  }, []);

  const resetPicks = useCallback(() => {
    setGames((prev) => {
      const cleared = prev.map((g) => ({
        ...g,
        winnerId: null,
        scoreHome: null,
        scoreAway: null,
      }));
      return reconcile(cleared);
    });
  }, []);

  const hasPicks = useMemo(() => games.some((g) => g.winnerId !== null), [games]);

  return { games, pickWinner, resetPicks, hasPicks };
}
