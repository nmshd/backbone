import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityDetailsMessagesComponent } from "./identity-details-messages.component";

describe("IdentityDetailsMessagesComponent", function () {
    let component: IdentityDetailsMessagesComponent;
    let fixture: ComponentFixture<IdentityDetailsMessagesComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [IdentityDetailsMessagesComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityDetailsMessagesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
