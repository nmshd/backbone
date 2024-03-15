import { ComponentFixture, TestBed } from "@angular/core/testing";

import { DeletionProcessDetailsComponent } from "./deletion-process-details.component";

describe("DeletionProcessDetailsComponent", () => {
    let component: DeletionProcessDetailsComponent;
    let fixture: ComponentFixture<DeletionProcessDetailsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [DeletionProcessDetailsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(DeletionProcessDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
