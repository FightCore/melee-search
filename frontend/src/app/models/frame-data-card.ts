import { SearchResultCard } from './search-result-card';

export interface FrameDataCard extends SearchResultCard {
  character: string;
  move: string;
  start: number;
  end: number;
  totalFrames: number;
  iasa: number;
  damage: string;
  baseKnockback: number;
  knockbackGrowth: number;
  angle: number;
  setKnockback: number;
  hitboxId?: string;
  notes?: string;
}
