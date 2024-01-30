import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CreateClientComponent } from "./create-client-dialog.component";

describe("CreateClientComponent", () => {
    let component: CreateClientComponent;
    let fixture: ComponentFixture<CreateClientComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CreateClientComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(CreateClientComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
