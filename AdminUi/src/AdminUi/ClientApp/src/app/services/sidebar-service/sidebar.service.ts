import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: "root"
})
export class SidebarService {
    private extended: boolean;
    private mobile: boolean;

    public constructor(breakpointObserver: BreakpointObserver) {
        this.mobile = false;

        breakpointObserver.observe([Breakpoints.XSmall, Breakpoints.Small]).subscribe((result) => {
            if (result.matches) {
                this.mobile = true;
            } else {
                this.mobile = false;
            }
        });

        this.extended = !this.isMobile();
    }

    public open(): void {
        this.extended = true;
    }

    public close(): void {
        this.extended = false;
    }

    public toggle(): void {
        this.extended = !this.extended;
    }

    public isOpen(): boolean {
        return this.extended;
    }

    public isMobile(): boolean {
        return this.mobile;
    }
}
