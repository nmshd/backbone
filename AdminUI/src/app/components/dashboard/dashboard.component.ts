import { Component } from '@angular/core';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent {
    header: string;
    dashboardOverviewPanels: DashboardOverviewPanel[];

    constructor() {
        this.header = '';
        this.dashboardOverviewPanels = [];
    }

    ngOnInit() {
        this.header = 'Dashboard';
        this.dashboardOverviewPanels = [
            {
                routerLink: '/identities',
                classLabel: 'identities',
                icon: 'pi-eye',
                header: 'Identities',
            },
            {
                routerLink: '/tiers',
                classLabel: 'identities',
                icon: 'pi-sort-amount-up-alt',
                header: 'Tiers',
            },
        ];
    }
}

interface DashboardOverviewPanel {
    routerLink: string;
    classLabel: string;
    icon: string;
    header: string;
}
