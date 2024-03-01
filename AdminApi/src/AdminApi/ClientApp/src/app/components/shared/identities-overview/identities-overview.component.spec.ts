import { ComponentFixture, TestBed } from "@angular/core/testing";

import { IdentitiesOverviewComponent } from "./identities-overview.component";

describe("IdentitiesOverviewComponent", function () {
    let component: IdentitiesOverviewComponent;
    let fixture: ComponentFixture<IdentitiesOverviewComponent>;

    beforeEach(function () {
        TestBed.configureTestingModule({
            declarations: [IdentitiesOverviewComponent]
        });
        fixture = TestBed.createComponent(IdentitiesOverviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
