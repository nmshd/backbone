import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DpAuditLogsDetailsComponent } from './dp-audit-logs-details.component';

describe('DpAuditLogsDetailsComponent', () => {
  let component: DpAuditLogsDetailsComponent;
  let fixture: ComponentFixture<DpAuditLogsDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DpAuditLogsDetailsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DpAuditLogsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
