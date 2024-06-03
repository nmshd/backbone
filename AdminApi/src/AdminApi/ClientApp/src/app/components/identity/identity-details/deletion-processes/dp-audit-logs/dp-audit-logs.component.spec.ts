import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DpAuditLogsComponent } from './dp-audit-logs.component';

describe('DpAuditLogsComponent', () => {
  let component: DpAuditLogsComponent;
  let fixture: ComponentFixture<DpAuditLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DpAuditLogsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DpAuditLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
