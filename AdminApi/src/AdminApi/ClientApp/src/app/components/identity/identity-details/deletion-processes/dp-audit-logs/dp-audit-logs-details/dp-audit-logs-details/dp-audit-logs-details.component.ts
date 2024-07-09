import { Component, OnInit } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { DeletionProcessAuditLog, IdentityService } from "src/app/services/identity-service/identity.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-dp-audit-logs-details",
    templateUrl: "./dp-audit-logs-details.component.html",
    styleUrls: ["./dp-audit-logs-details.component.css"]
})
export class DeletionProcessAuditLogsDetailsComponent implements OnInit {
    public identityDeletionProcessID: string;
    public identityAddress: string;

    public header: string;
    public headerDeletionProcessAuditLog: string;
    public headerDeletionProcessAuditLogDescription: string;

    public loading: boolean;
    public deletionProcessesAuditLogTableDisplayedColumns: string[];

    public identityDeletionProcessAuditLogs: DeletionProcessAuditLog[] = [];

    public constructor(
        private readonly identityService: IdentityService,
        private readonly snackBar: MatSnackBar,
        private readonly activatedRoute: ActivatedRoute
    ) {
        this.header = "Deletion Process Details";
        this.headerDeletionProcessAuditLog = "Deletion Process Audit Logs";
        this.headerDeletionProcessAuditLogDescription = "View deletion process audit logs for Identity.";
        this.loading = false;
        this.deletionProcessesAuditLogTableDisplayedColumns = ["id", "createdAt", "message", "oldStatus", "newStatus"];

        this.identityDeletionProcessID = "";
        this.identityAddress = "";
    }

    public ngOnInit(): void {
        this.activatedRoute.params.subscribe((params) => {
            this.identityAddress = params["address"];
            this.loadIdentityDeletionProcessAuditLogs();
        });
    }

    private loadIdentityDeletionProcessAuditLogs(): void {
        this.loading = true;
        this.identityService.getDeletionProcessAuditLogsOfIdentity(this.identityAddress.trim()).subscribe({
            next: (data: HttpResponseEnvelope<DeletionProcessAuditLog[]>) => {
                this.identityDeletionProcessAuditLogs = data.result;
                this.loading = false;
            },
            error: (err: any) => {
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
                this.loading = false;
            }
        });
    }

    public styleStatus(status: string): string {
        if (status === "WaitingForApproval") return "Waiting for Approval";
        return status;
    }

    public getFormattedMessage(messageKey: string, additionalData: Record<string, string>): string {
        return this.identityService.getFormattedMessage(messageKey, additionalData);
    }
}
