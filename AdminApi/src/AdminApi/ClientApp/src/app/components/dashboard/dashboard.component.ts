import { Component } from "@angular/core";
import { SidebarService } from "src/app/services/sidebar-service/sidebar.service";

@Component({
    selector: "app-dashboard",
    templateUrl: "./dashboard.component.html",
    styleUrls: ["./dashboard.component.css"]
})
export class DashboardComponent {
    public header: string;
    public breakpoint: number;
    public dashboardOverviewPanels: DashboardOverviewPanel[];

    public constructor(private readonly sidebarService: SidebarService) {
        this.header = "";
        this.breakpoint = window.innerWidth <= 1150 ? 1 : window.innerWidth <= 1700 ? 2 : 3;
        this.dashboardOverviewPanels = [];
    }

    public ngOnInit(): void {
        this.header = "Dashboard";
        this.dashboardOverviewPanels = [
            {
                routerLink: "/identities",
                classLabel: "identities",
                icon: "badge",
                header: "Identities",
                description: "View a list of all existing Identities."
            },
            {
                routerLink: "/tiers",
                classLabel: "identities",
                icon: "clear_all",
                header: "Tiers",
                description: "List all of the application's existing Tiers and create new ones."
            },
            {
                routerLink: "/clients",
                classLabel: "identities",
                icon: "person",
                header: "Clients",
                description: "List all of the application's clients and create new ones."
            }
        ];
    }

    public onResize(): void {
        this.breakpoint = window.innerWidth <= 1150 ? 1 : window.innerWidth <= 1700 ? 2 : 3;
    }

    public isMobile(): boolean {
        return this.sidebarService.isMobile();
    }
}

interface DashboardOverviewPanel {
    routerLink: string;
    classLabel: string;
    icon: string;
    header: string;
    description: string;
}
