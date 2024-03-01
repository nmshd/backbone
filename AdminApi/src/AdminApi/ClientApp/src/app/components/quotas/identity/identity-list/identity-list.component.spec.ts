import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityListComponent } from "./identity-list.component";

describe("IdentityListComponent", function () {
    let component: IdentityListComponent;
    let fixture: ComponentFixture<IdentityListComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [IdentityListComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
