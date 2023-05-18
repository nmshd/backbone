import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent {
  sidebarOptions: SidebarOption[];

  isMobileLayout: boolean;

  constructor(private router: Router) {
    this.sidebarOptions = [];
    this.isMobileLayout = false;
  }

  ngOnInit() {
    this.sidebarOptions = [
      {
        routerLink: '/dashboard',
        icon: 'apps',
        label: 'Dashboard',
      },
      {
        routerLink: '/identities',
        icon: 'badge',
        label: 'Identities',
      },
      {
        routerLink: '/tiers',
        icon: 'clear_all',
        label: 'Tiers',
      },
    ];
  }
}

interface SidebarOption {
  routerLink: string;
  icon: string;
  label: string;
}
