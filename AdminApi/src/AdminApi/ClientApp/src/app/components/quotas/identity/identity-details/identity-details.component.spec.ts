import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityDetailsComponent } from "./identity-details.component";

describe("IdentityDetailsComponent", function () {
    let component: IdentityDetailsComponent;
    let fixture: ComponentFixture<IdentityDetailsComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [IdentityDetailsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
