import { Component } from '@angular/core';
import { formatDate } from '@angular/common';
import { SidebarService } from 'src/app/services/sidebar-service/sidebar.service';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.css'],
})
export class TopbarComponent {
  today: string;

  constructor(private sidebarService: SidebarService) {
    this.today = '';
  }

  ngOnInit() {
    this.today = formatDate(new Date(), 'd MMM y, EEEE', 'en-DE');
  }

  toggleSidebar() {
    this.sidebarService.toggle();
  }
}
