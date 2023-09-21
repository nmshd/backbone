import { Component } from "@angular/core";
import { AuthService } from "src/app/services/auth-service/auth.service";
import { SidebarService } from "src/app/services/sidebar-service/sidebar.service";

@Component({
    selector: "app-topbar",
    templateUrl: "./topbar.component.html",
    styleUrls: ["./topbar.component.css"]
})
export class TopbarComponent {
    public constructor(
        private readonly sidebarService: SidebarService,
        private readonly authService: AuthService
    ) {}

    public toggleSidebar(): void {
        this.sidebarService.toggle();
    }

    public async logout(): Promise<boolean> {
        return await this.authService.logout();
    }
}
