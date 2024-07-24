import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CreateClientDialogComponent } from "./create-client-dialog.component";

describe("CreateClientDialogComponent", function () {
    let component: CreateClientDialogComponent;
    let fixture: ComponentFixture<CreateClientDialogComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [CreateClientDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(CreateClientDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
