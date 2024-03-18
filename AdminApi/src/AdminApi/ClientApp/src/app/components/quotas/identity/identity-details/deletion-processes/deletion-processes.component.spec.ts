import { ComponentFixture, TestBed } from "@angular/core/testing";

import { DeletionProcessesComponent } from "./deletion-processes.component";

describe("DeletionProcessesComponent", function () {
    let component: DeletionProcessesComponent;
    let fixture: ComponentFixture<DeletionProcessesComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            imports: [DeletionProcessesComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(DeletionProcessesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
