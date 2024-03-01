import { ComponentFixture, TestBed } from "@angular/core/testing";

import { TopbarComponent } from "./topbar.component";

describe("TopbarComponent", function () {
    let component: TopbarComponent;
    let fixture: ComponentFixture<TopbarComponent>;

    beforeEach(async function () {
        await TestBed.configureTestingModule({
            declarations: [TopbarComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(TopbarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", async function () {
        await expect(component).toBeTruthy();
    });
});
