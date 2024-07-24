import { ComponentFixture, TestBed } from "@angular/core/testing";

import { LoginComponent } from "./login.component";

describe("LoginComponent", function () {
    let component: LoginComponent;
    let fixture: ComponentFixture<LoginComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [LoginComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(LoginComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
