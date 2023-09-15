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
    title = "AdminUI";
    isLoggedIn$?: Observable<boolean>;

    constructor(
        private sidebarService: SidebarService,
        private authService: AuthService,
        private snackBar: MatSnackBar,
        private xsrfService: XSRFService
    ) {}

    ngOnInit() {
        this.isLoggedIn$ = this.authService.isLoggedIn;
        this.xsrfService.loadAndStoreXSRFToken();
    }

    closeSidebar() {
        this.sidebarService.close();
    }

    isSidebarOpen(): boolean {
        return this.sidebarService.isOpen();
    }

    isMobile(): boolean {
        return this.sidebarService.isMobile();
    }

    changeOfRoute(): void {
        this.snackBar.dismiss();
    }
}
