import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StartDeletionProcessDialogComponent } from './start-deletion-process-dialog.component';

describe('StartDeletionProcessDialogComponent', () => {
  let component: StartDeletionProcessDialogComponent;
  let fixture: ComponentFixture<StartDeletionProcessDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StartDeletionProcessDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StartDeletionProcessDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
