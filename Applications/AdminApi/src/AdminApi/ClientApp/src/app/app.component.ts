import { Component, OnInit } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Observable } from "rxjs";
import { AuthService } from "./services/auth-service/auth.service";
import { SidebarService } from "./services/sidebar-service/sidebar.service";
import { XSRFService } from "./services/xsrf-service/xsrf.service";

@Component({
    selector: "app-root",
    templateUrl: "./app.component.html",
    styleUrls: ["./app.component.css"]
})
export class AppComponent implements OnInit {
    public title = "AdminUI";
    public isLoggedIn$?: Observable<boolean>;

    public constructor(
        private readonly sidebarService: SidebarService,
        private readonly authService: AuthService,
        private readonly snackBar: MatSnackBar,
        private readonly xsrfService: XSRFService
    ) {}

    public ngOnInit(): void {
        this.isLoggedIn$ = this.authService.isLoggedIn;
        this.xsrfService.loadAndStoreXSRFToken();
    }

    public closeSidebar(): void {
        this.sidebarService.close();
    }

    public isSidebarOpen(): boolean {
        return this.sidebarService.isOpen();
    }

    public isMobile(): boolean {
        return this.sidebarService.isMobile();
    }

    public changeOfRoute(): void {
        this.snackBar.dismiss();
    }
}
