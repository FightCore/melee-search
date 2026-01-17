import { SearchResultCard } from './search-result-card';

export interface FrameDataCard extends SearchResultCard {
  character: string;
  move: string;
  start: number;
  end: number;
  totalFrames: number;
  iasa: number;
  damage: number[];
  baseKnockback: number[];
  knockbackGrowth: number[];
  setKnockback: number[];
  angle: number[];
  notes?: string;
}
