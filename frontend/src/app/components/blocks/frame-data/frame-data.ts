import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { FrameDataCard } from '../../../models/frame-data-card';

@Component({
  selector: 'app-frame-data',
  templateUrl: './frame-data.html',
  styleUrl: './frame-data.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FrameData {
  card = input.required<FrameDataCard>();
}
