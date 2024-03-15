import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentityDetailsDeletionProcessesComponent } from "./identity-details-deletion-processes.component";

describe("IdentityDetailsDeletionProcessesComponent", () => {
    let component: IdentityDetailsDeletionProcessesComponent;
    let fixture: ComponentFixture<IdentityDetailsDeletionProcessesComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [IdentityDetailsDeletionProcessesComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(IdentityDetailsDeletionProcessesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
