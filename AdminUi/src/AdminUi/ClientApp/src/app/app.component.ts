import { Component, OnInit } from '@angular/core';
import { SidebarService } from './services/sidebar-service/sidebar.service';
import { Observable } from 'rxjs';
import { AuthService } from './services/auth-service/auth.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
    title = 'AdminUI';
    isLoggedIn$?: Observable<boolean>;

    constructor(private sidebarService: SidebarService,
        private authService: AuthService) { }

    ngOnInit() {
        this.isLoggedIn$ = this.authService.isLoggedIn;
    }

    closeSidebar() {
        this.sidebarService.close();
    }

    isSidebarOpen(): boolean {
        return this.sidebarService.isOpen();
    }

    isMobile(): boolean {
        return this.sidebarService.isMobile();
    }
}
