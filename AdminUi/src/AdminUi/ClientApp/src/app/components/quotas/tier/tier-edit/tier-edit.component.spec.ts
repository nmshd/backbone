import { ComponentFixture, TestBed } from "@angular/core/testing";

import { TierEditComponent } from "./tier-edit.component";

describe("TierEditComponent", function () {
    let component: TierEditComponent;
    let fixture: ComponentFixture<TierEditComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [TierEditComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(TierEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
