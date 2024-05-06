import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityDetailsMessageRecipientsDialogComponent } from "./identity-details-message-recipients-dialog.component";

describe("IdentityDetailsMessageRecipientsDialogComponent", function () {
    let component: IdentityDetailsMessageRecipientsDialogComponent;
    let fixture: ComponentFixture<IdentityDetailsMessageRecipientsDialogComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            imports: [IdentityDetailsMessageRecipientsDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityDetailsMessageRecipientsDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
