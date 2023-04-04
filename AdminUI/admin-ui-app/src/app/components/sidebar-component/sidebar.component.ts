import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SidebarService } from 'src/app/services/sidebar-service/sidebar.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent {
  sidebarOptions: SidebarOption[];

  isMobileLayout: boolean;

  constructor(private sidebarService: SidebarService, private router: Router) {
    this.sidebarOptions = [];
    this.isMobileLayout = false;
  }

  ngOnInit() {
    this.sidebarOptions = [
      {
        routerLink: '/dashboard',
        icon: 'pi-th-large',
        label: 'Dashboard',
      },
      {
        routerLink: '/identities',
        icon: 'pi-eye',
        label: 'Identities',
      },
      {
        routerLink: '/tiers',
        icon: 'pi-sort-amount-up-alt',
        label: 'Tiers',
      },
    ];
    this.setupSidebar();
  }

  setupSidebar() {
    let mobileSize = this.sidebarService.getMobileSize();

    this.isMobileLayout = window.screen.width <= mobileSize;
    window.onresize = () => {
      if (this.isMobileLayout != window.screen.width <= mobileSize) {
        this.closeSidebar();
        setTimeout(() => {
          this.isMobileLayout = window.screen.width <= mobileSize;
        }, 300);
      }
    };

    this.router.events.subscribe(() => {
      if (this.isMobileLayout) {
        this.closeSidebar();
      }
    });
  }

  showSidebar() {
    return this.sidebarService.isOpen() || !this.isMobileLayout;
  }

  extendedSidebar() {
    return this.sidebarService.isOpen();
  }

  closeSidebar() {
    return this.sidebarService.close();
  }

  openSidebar() {
    return this.sidebarService.open();
  }
}

interface SidebarOption {
  routerLink: string;
  icon: string;
  label: string;
}
