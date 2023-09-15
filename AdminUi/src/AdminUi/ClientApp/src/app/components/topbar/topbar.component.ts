import { Component } from "@angular/core";
import { AuthService } from "src/app/services/auth-service/auth.service";
import { SidebarService } from "src/app/services/sidebar-service/sidebar.service";

@Component({
    selector: "app-topbar",
    templateUrl: "./topbar.component.html",
    styleUrls: ["./topbar.component.css"]
})
export class TopbarComponent {
    constructor(
        private readonly sidebarService: SidebarService,
        private readonly authService: AuthService
    ) {}

    toggleSidebar(): void {
        this.sidebarService.toggle();
    }

    logout(): void {
        this.authService.logout();
    }
}
