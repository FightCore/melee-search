import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrameData } from './frame-data';

describe('FrameData', () => {
  let component: FrameData;
  let fixture: ComponentFixture<FrameData>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FrameData]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrameData);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
