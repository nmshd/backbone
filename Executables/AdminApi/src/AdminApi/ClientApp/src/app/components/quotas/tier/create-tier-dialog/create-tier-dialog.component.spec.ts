import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CreateTierDialogComponent } from "./create-tier-dialog.component";

describe("CreateTierDialogComponent", function () {
    let component: CreateTierDialogComponent;
    let fixture: ComponentFixture<CreateTierDialogComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [CreateTierDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(CreateTierDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
