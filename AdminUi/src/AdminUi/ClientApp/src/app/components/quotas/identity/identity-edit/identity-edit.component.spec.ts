import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityEditComponent } from "./identity-edit.component";

describe("IdentityEditComponent", () => {
    let component: IdentityEditComponent;
    let fixture: ComponentFixture<IdentityEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [IdentityEditComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
