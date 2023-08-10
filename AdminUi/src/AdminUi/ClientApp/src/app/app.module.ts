import { DATE_PIPE_DEFAULT_OPTIONS } from "@angular/common";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { ClipboardModule } from "@angular/cdk/clipboard";
import { LayoutModule } from "@angular/cdk/layout";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatChipsModule } from "@angular/material/chips";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTooltipModule } from "@angular/material/tooltip";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ChangeSecretDialogComponent } from "./components/client/change-secret-dialog/change-secret-dialog.component";
import { ClientEditComponent } from "./components/client/client-edit/client-edit.component";
import { ClientListComponent } from "./components/client/client-list/client-list.component";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { PageNotFoundComponent } from "./components/error/page-not-found/page-not-found.component";
import { AssignQuotasDialogComponent } from "./components/quotas/assign-quotas-dialog/assign-quotas-dialog.component";
import { IdentityEditComponent } from "./components/quotas/identity/identity-edit/identity-edit.component";
import { IdentityListComponent } from "./components/quotas/identity/identity-list/identity-list.component";
import { TierEditComponent } from "./components/quotas/tier/tier-edit/tier-edit.component";
import { TierListComponent } from "./components/quotas/tier/tier-list/tier-list.component";
import { ConfirmationDialogComponent } from "./components/shared/confirmation-dialog/confirmation-dialog.component";
import { LoginComponent } from "./components/shared/login/login.component";
import { SidebarComponent } from "./components/sidebar/sidebar.component";
import { TopbarComponent } from "./components/topbar/topbar.component";
import { SidebarService } from "./services/sidebar-service/sidebar.service";
import { ApiKeyInterceptor } from "./shared/interceptors/api-key.interceptor";
import { XSRFInterceptor } from "./shared/interceptors/xsrf.interceptor";

@NgModule({
    declarations: [
        AppComponent,
        DashboardComponent,
        PageNotFoundComponent,
        SidebarComponent,
        TopbarComponent,
        IdentityListComponent,
        TierListComponent,
        TierEditComponent,
        ClientListComponent,
        IdentityEditComponent,
        ClientEditComponent,
        AssignQuotasDialogComponent,
        ConfirmationDialogComponent,
        LoginComponent,
        ChangeSecretDialogComponent
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        ClipboardModule,
        HttpClientModule,
        MatCardModule,
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        MatSidenavModule,
        MatCheckboxModule,
        MatListModule,
        MatGridListModule,
        MatTableModule,
        MatPaginatorModule,
        MatProgressSpinnerModule,
        MatSnackBarModule,
        MatInputModule,
        MatFormFieldModule,
        MatExpansionModule,
        MatTooltipModule,
        LayoutModule,
        MatDialogModule,
        MatSelectModule,
        MatChipsModule
    ],
    providers: [
        SidebarService,
        {
            provide: DATE_PIPE_DEFAULT_OPTIONS,
            useValue: { dateFormat: "dd.MM.yyyy HH:mm:ss" }
        },
        { provide: HTTP_INTERCEPTORS, useClass: ApiKeyInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: XSRFInterceptor, multi: true}
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
