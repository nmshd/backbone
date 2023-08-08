import { Component } from "@angular/core";
import { SidebarService } from "src/app/services/sidebar-service/sidebar.service";

@Component({
    selector: "app-sidebar",
    templateUrl: "./sidebar.component.html",
    styleUrls: ["./sidebar.component.css"]
})
export class SidebarComponent {
    sidebarOptions: SidebarOption[];

    constructor(private sidebarService: SidebarService) {
        this.sidebarOptions = [];
    }

    ngOnInit() {
        this.sidebarOptions = [
            {
                routerLink: "/dashboard",
                icon: "apps",
                label: "Dashboard"
            },
            {
                routerLink: "/identities",
                icon: "badge",
                label: "Identities"
            },
            {
                routerLink: "/tiers",
                icon: "clear_all",
                label: "Tiers"
            },
            {
                routerLink: "/clients",
                icon: "person",
                label: "Clients"
            }
        ];
    }

    isMobile(): boolean {
        return this.sidebarService.isMobile();
    }
}

interface SidebarOption {
    routerLink: string;
    icon: string;
    label: string;
}
