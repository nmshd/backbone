import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityEditRelationshipsComponent } from "./identity-edit-relationships.component";

describe("IdentityEditRelationshipsComponent", function () {
    let component: IdentityEditRelationshipsComponent;
    let fixture: ComponentFixture<IdentityEditRelationshipsComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [IdentityEditRelationshipsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityEditRelationshipsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
