import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { RippleModule } from 'primeng/ripple';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ImageModule } from 'primeng/image';
import { FormsModule } from '@angular/forms';
import { AccordionModule } from 'primeng/accordion';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SidebarModule } from 'primeng/sidebar';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { IdentityListComponent } from './components/quotas/identity/identity-list/identity-list.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { PageNotFoundComponent } from './components/error/page-not-found/page-not-found.component';
import { TierListComponent } from './components/quotas/tier/tier-list/tier-list.component';
import { TierEditComponent } from './components/quotas/tier/tier-edit/tier-edit.component';
import { TopbarComponent } from './components/topbar/topbar.component';
import { SidebarService } from './services/sidebar-service/sidebar.service';

@NgModule({
    declarations: [
        AppComponent,
        IdentityListComponent,
        DashboardComponent,
        SidebarComponent,
        PageNotFoundComponent,
        TierListComponent,
        TierEditComponent,
        TopbarComponent,
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        HttpClientModule,
        RippleModule,
        CardModule,
        TableModule,
        ButtonModule,
        InputTextModule,
        ImageModule,
        FormsModule,
        AccordionModule,
        ToastModule,
        ProgressSpinnerModule,
        SidebarModule,
    ],
    providers: [SidebarService],
    bootstrap: [AppComponent],
})
export class AppModule {}
