import { CommonModule, DATE_PIPE_DEFAULT_OPTIONS } from "@angular/common";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { ClipboardModule } from "@angular/cdk/clipboard";
import { LayoutModule } from "@angular/cdk/layout";
import { LoggerModule, NgxLoggerLevel, TOKEN_LOGGER_SERVER_SERVICE, TOKEN_LOGGER_WRITER_SERVICE } from "ngx-logger";

import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatChipsModule } from "@angular/material/chips";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTooltipModule } from "@angular/material/tooltip";

import { MatSortModule } from "@angular/material/sort";
import { environment } from "src/environments/environment";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ChangeSecretDialogComponent } from "./components/client/change-secret-dialog/change-secret-dialog.component";
import { ClientEditComponent } from "./components/client/client-edit/client-edit.component";
import { ClientListComponent } from "./components/client/client-list/client-list.component";
import { CreateClientDialogComponent } from "./components/client/create-client-dialog/create-client-dialog.component";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { PageNotFoundComponent } from "./components/error/page-not-found/page-not-found.component";
import { DeletionProcessesComponent } from "./components/identity/identity-details/deletion-processes/deletion-processes.component";
import { CancelDeletionProcessDialogComponent } from "./components/identity/identity-details/deletion-processes/dp-details/cancel-dp-dialog/cancel-dp-dialog.component";
import { DeletionProcessDetailsComponent } from "./components/identity/identity-details/deletion-processes/dp-details/dp-details.component";
import { IdentityDetailsRelationshipsComponent } from "./components/identity/identity-details/identity-details-relationships/identity-details-relationships.component";
import { IdentityDetailsComponent } from "./components/identity/identity-details/identity-details.component";
import { StartDeletionProcessDialogComponent } from "./components/identity/identity-details/start-deletion-process-dialog/start-deletion-process-dialog.component";
import { IdentityListComponent } from "./components/identity/identity-list/identity-list.component";
import { AssignQuotasDialogComponent } from "./components/quotas/assign-quotas-dialog/assign-quotas-dialog.component";
import { CreateTierDialogComponent } from "./components/quotas/tier/create-tier-dialog/create-tier-dialog.component";
import { TierEditComponent } from "./components/quotas/tier/tier-edit/tier-edit.component";
import { TierListComponent } from "./components/quotas/tier/tier-list/tier-list.component";
import { BreadcrumbComponent } from "./components/shared/breadcrumb/breadcrumb.component";
import { ConfirmationDialogComponent } from "./components/shared/confirmation-dialog/confirmation-dialog.component";
import { IdentitiesOverviewComponent } from "./components/shared/identities-overview/identities-overview.component";
import { LoginComponent } from "./components/shared/login/login.component";
import { SidebarComponent } from "./components/sidebar/sidebar.component";
import { TopbarComponent } from "./components/topbar/topbar.component";
import { LoggerServerService } from "./services/logger-server-service/logger-server.service";
import { LoggerWriterService } from "./services/logger-writer-service/logger-writer.service";
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
        IdentityDetailsComponent,
        TierListComponent,
        TierEditComponent,
        CreateTierDialogComponent,
        ClientListComponent,
        ClientEditComponent,
        CreateClientDialogComponent,
        AssignQuotasDialogComponent,
        ConfirmationDialogComponent,
        LoginComponent,
        ChangeSecretDialogComponent,
        IdentitiesOverviewComponent,
        IdentityDetailsRelationshipsComponent,
        BreadcrumbComponent,
        DeletionProcessesComponent,
        BreadcrumbComponent,
        DeletionProcessDetailsComponent,
        CancelDeletionProcessDialogComponent,
        StartDeletionProcessDialogComponent
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        BrowserModule,
        CommonModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        ClipboardModule,
        HttpClientModule,
        LoggerModule.forRoot(
            {
                serverLoggingUrl: `${environment.apiUrl}/Logs`,
                level: environment.production ? NgxLoggerLevel.INFO : NgxLoggerLevel.TRACE,
                serverLogLevel: NgxLoggerLevel.ERROR,
                enableSourceMaps: true
            },
            {
                writerProvider: {
                    provide: TOKEN_LOGGER_WRITER_SERVICE,
                    useClass: LoggerWriterService
                },
                serverProvider: {
                    provide: TOKEN_LOGGER_SERVER_SERVICE,
                    useClass: LoggerServerService
                }
            }
        ),
        MatCardModule,
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        MatSidenavModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatCheckboxModule,
        MatSortModule,
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
        MatChipsModule,
        MatProgressBarModule
    ],
    providers: [
        SidebarService,
        {
            provide: DATE_PIPE_DEFAULT_OPTIONS,
            useValue: { dateFormat: "dd.MM.yyyy HH:mm:ss" }
        },
        { provide: HTTP_INTERCEPTORS, useClass: ApiKeyInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: XSRFInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
