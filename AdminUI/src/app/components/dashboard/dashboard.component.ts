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
        icon: 'badge',
        header: 'Identities',
      },
      {
        routerLink: '/tiers',
        classLabel: 'identities',
        icon: 'clear_all',
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
