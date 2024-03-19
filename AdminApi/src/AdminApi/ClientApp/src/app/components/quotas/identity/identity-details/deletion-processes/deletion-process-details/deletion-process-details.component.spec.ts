import { ComponentFixture, TestBed } from "@angular/core/testing";

import { DeletionProcessDetailsComponent } from "./deletion-process-details.component";

describe("DeletionProcessDetailsComponent", function () {
    let component: DeletionProcessDetailsComponent;
    let fixture: ComponentFixture<DeletionProcessDetailsComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            imports: [DeletionProcessDetailsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(DeletionProcessDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
