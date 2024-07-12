import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ClientListComponent } from "./client-list.component";

describe("ClientListComponent", function () {
    let component: ClientListComponent;
    let fixture: ComponentFixture<ClientListComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [ClientListComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ClientListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
