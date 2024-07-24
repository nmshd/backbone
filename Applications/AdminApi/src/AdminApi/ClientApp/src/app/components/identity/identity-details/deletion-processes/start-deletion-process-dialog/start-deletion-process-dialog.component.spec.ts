import { ComponentFixture, TestBed } from "@angular/core/testing";

import { StartDeletionProcessDialogComponent } from "./start-deletion-process-dialog.component";

describe("StartDeletionProcessDialogComponent", function () {
    let component: StartDeletionProcessDialogComponent;
    let fixture: ComponentFixture<StartDeletionProcessDialogComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            imports: [StartDeletionProcessDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(StartDeletionProcessDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
