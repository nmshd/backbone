import { ComponentFixture, TestBed } from "@angular/core/testing";

import { DownloadMessageDialogComponent } from "./download-message-dialog.component";

describe("DownloadMessageDialogComponent", function () {
    let component: DownloadMessageDialogComponent;
    let fixture: ComponentFixture<DownloadMessageDialogComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [DownloadMessageDialogComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(DownloadMessageDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
