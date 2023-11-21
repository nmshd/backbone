import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityDetailsRelationshipsComponent } from "./identity-details-relationships.component";

describe("IdentityDetailsRelationshipsComponent", function () {
    let component: IdentityDetailsRelationshipsComponent;
    let fixture: ComponentFixture<IdentityDetailsRelationshipsComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [IdentityDetailsRelationshipsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityDetailsRelationshipsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
