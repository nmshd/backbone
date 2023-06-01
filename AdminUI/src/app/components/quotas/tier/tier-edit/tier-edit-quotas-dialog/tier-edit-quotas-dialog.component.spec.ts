import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TierEditQuotasDialogComponent } from './tier-edit-quotas-dialog.component';

describe('TierEditQuotasDialogComponent', () => {
  let component: TierEditQuotasDialogComponent;
  let fixture: ComponentFixture<TierEditQuotasDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TierEditQuotasDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TierEditQuotasDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
