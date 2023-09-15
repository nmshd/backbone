import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityEditComponent } from "./identity-edit.component";

describe("IdentityEditComponent", function() {
    let component: IdentityEditComponent;
    let fixture: ComponentFixture<IdentityEditComponent>;

    beforeEach(async function() {
        await TestBed.configureTestingModule({
            declarations: [IdentityEditComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", function() {
        expect(component).toBeTruthy();
    });
});
