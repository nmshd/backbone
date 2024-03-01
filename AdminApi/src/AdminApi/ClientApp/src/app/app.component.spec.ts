import { TestBed } from "@angular/core/testing";
import { RouterTestingModule } from "@angular/router/testing";
import { AppComponent } from "./app.component";

describe("AppComponent", function () {
    beforeEach(async function () {
        await TestBed.configureTestingModule({
            imports: [RouterTestingModule],
            declarations: [AppComponent]
        }).compileComponents();
    });

    it("should create the app", async function () {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.componentInstance;
        await expect(app).toBeTruthy();
    });

    it("should have as title 'AdminUI'", async function () {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.componentInstance;
        await expect(app.title).toEqual("AdminUI");
    });

    it("should render title", async function () {
        const fixture = TestBed.createComponent(AppComponent);
        fixture.detectChanges();
        const compiled = fixture.nativeElement as HTMLElement;
        await expect(compiled.querySelector(".content span")?.textContent).toContain("AdminUI app is running!");
    });
});
