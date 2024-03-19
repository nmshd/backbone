import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CancelDeletionProcessDialogComponent } from "./cancel-deletion-process-dialog.component";

describe("CancelDeletionProcessDialogComponent", function() {
    let component: CancelDeletionProcessDialogComponent;
    let fixture: ComponentFixture<CancelDeletionProcessDialogComponent>;

    beforeEach(async function() {
        await TestBed.configureTestingModule({
            imports: [CancelDeletionProcessDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(CancelDeletionProcessDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function() {
        await expect(component).toBeTruthy();
    });
});
