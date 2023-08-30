import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { DATE_PIPE_DEFAULT_OPTIONS } from "@angular/common";

import { LoggerModule, NgxLoggerLevel, TOKEN_LOGGER_SERVER_SERVICE, TOKEN_LOGGER_WRITER_SERVICE } from "ngx-logger";

import { MatCardModule } from "@angular/material/card";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatListModule } from "@angular/material/list";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatTableModule } from "@angular/material/table";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatTooltipModule } from "@angular/material/tooltip";
import { LayoutModule } from "@angular/cdk/layout";
import { ClipboardModule } from "@angular/cdk/clipboard";
import { MatDialogModule } from "@angular/material/dialog";
import { MatSelectModule } from "@angular/material/select";
import { MatChipsModule } from "@angular/material/chips";
import { MatCheckboxModule } from "@angular/material/checkbox";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { SidebarService } from "./services/sidebar-service/sidebar.service";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { PageNotFoundComponent } from "./components/error/page-not-found/page-not-found.component";
import { SidebarComponent } from "./components/sidebar/sidebar.component";
import { TopbarComponent } from "./components/topbar/topbar.component";
import { IdentityListComponent } from "./components/quotas/identity/identity-list/identity-list.component";
import { TierListComponent } from "./components/quotas/tier/tier-list/tier-list.component";
import { TierEditComponent } from "./components/quotas/tier/tier-edit/tier-edit.component";
import { ClientListComponent } from "./components/client/client-list/client-list.component";
import { IdentityEditComponent } from "./components/quotas/identity/identity-edit/identity-edit.component";
import { ClientEditComponent } from "./components/client/client-edit/client-edit.component";
import { AssignQuotasDialogComponent } from "./components/quotas/assign-quotas-dialog/assign-quotas-dialog.component";
import { ConfirmationDialogComponent } from "./components/shared/confirmation-dialog/confirmation-dialog.component";
import { LoginComponent } from "./components/shared/login/login.component";
import { ApiKeyInterceptor } from "./shared/interceptors/api-key.interceptor";
import { ChangeSecretDialogComponent } from "./components/client/change-secret-dialog/change-secret-dialog.component";
import { environment } from "src/environments/environment";
import { LoggerServerService } from "./services/logger-server-service/logger-server.service";
import { LoggerWriterService } from "./services/logger-writer-service/logger-writer.service";

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
        LoggerModule.forRoot(
            {
                serverLoggingUrl: environment.apiUrl + "/Logs",
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
        { provide: HTTP_INTERCEPTORS, useClass: ApiKeyInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
