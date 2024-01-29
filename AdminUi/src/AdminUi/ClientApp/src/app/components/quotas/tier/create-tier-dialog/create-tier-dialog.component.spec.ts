import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CreateTierDialogComponent } from "./create-tier-dialog.component";

describe("CreateTierDialogComponent", () => {
    let component: CreateTierDialogComponent;
    let fixture: ComponentFixture<CreateTierDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CreateTierDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(CreateTierDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
