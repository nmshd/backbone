import { ComponentFixture, TestBed } from "@angular/core/testing";

import { DeletionProcessAuditLogsDetailsComponent } from "./dp-audit-logs-details.component";

describe("DeletionProcessAuditLogsDetailsComponent", () => {
    let component: DeletionProcessAuditLogsDetailsComponent;
    let fixture: ComponentFixture<DeletionProcessAuditLogsDetailsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [DeletionProcessAuditLogsDetailsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(DeletionProcessAuditLogsDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async () => {
        await expect(component).toBeTruthy();
    });
});
