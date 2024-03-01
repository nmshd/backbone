import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ClientEditComponent } from "./client-edit.component";

describe("ClientEditComponent", function () {
    let component: ClientEditComponent;
    let fixture: ComponentFixture<ClientEditComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [ClientEditComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ClientEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
