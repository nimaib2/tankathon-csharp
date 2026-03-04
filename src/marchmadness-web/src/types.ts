export interface Team {
  id: string;
  name: string;
  shortName: string;
  conference: string;
  seed: number;
  regionId: string;
  isFirstFour: boolean;
  firstFourSlot?: string;
}

export interface Region {
  id: string;
  name: string;
}

export interface BracketGame {
  id: string;
  round: number;
  regionId: string | null;
  homeTeamId: string | null;
  awayTeamId: string | null;
  winnerId: string | null;
  scoreHome: number | null;
  scoreAway: number | null;
  gameIndex: number;
  sourceGame1Id: string | null;
  sourceGame2Id: string | null;
}

export interface Bracket {
  season: string;
  source: string | null;
  regions: Region[];
  teams: Team[];
  games: BracketGame[];
}
