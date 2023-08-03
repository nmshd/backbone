import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ChangeSecretDialogComponent } from "./change-secret-dialog.component";

describe("ChangeSecretDialogComponent", () => {
    let component: ChangeSecretDialogComponent;
    let fixture: ComponentFixture<ChangeSecretDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ChangeSecretDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ChangeSecretDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
