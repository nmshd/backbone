import { ComponentFixture, TestBed } from "@angular/core/testing";

import { DeletionProcessAuditLogsComponent } from "./dp-audit-logs.component";

describe("DeletionProcessAuditLogsComponent", () => {
    let component: DeletionProcessAuditLogsComponent;
    let fixture: ComponentFixture<DeletionProcessAuditLogsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [DeletionProcessAuditLogsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(DeletionProcessAuditLogsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async () => {
        await expect(component).toBeTruthy();
    });
});
