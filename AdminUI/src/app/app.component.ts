import { Component } from '@angular/core';
import { SidebarService } from './services/sidebar-service/sidebar.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
})
export class AppComponent {
    title = 'AdminUI';

    constructor(private sidebarService: SidebarService) {}

    closeSidebar() {
        this.sidebarService.close();
    }

    showSidebar(): boolean {
        return this.sidebarService.isOpen();
    }

    isMobile(): boolean {
        return this.sidebarService.isMobile();
    }
}
