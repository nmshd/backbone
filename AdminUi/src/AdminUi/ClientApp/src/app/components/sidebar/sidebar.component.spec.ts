import { ComponentFixture, TestBed } from "@angular/core/testing";

import { SidebarComponent } from "./sidebar.component";

describe("SidebarComponent", function() {
    let component: SidebarComponent;
    let fixture: ComponentFixture<SidebarComponent>;

    beforeEach(async function() {
        await TestBed.configureTestingModule({
            declarations: [SidebarComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(SidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", function() {
        expect(component).toBeTruthy();
    });
});
