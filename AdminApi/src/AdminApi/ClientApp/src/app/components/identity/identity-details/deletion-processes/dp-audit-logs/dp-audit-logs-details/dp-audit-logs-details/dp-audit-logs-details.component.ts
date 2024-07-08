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

    public getFormattedMessage(messageKey: string, additionalData: { [key: string]: string }): string {
        let messageTemplate: string;

        switch (messageKey) {
            case "StartedByOwner":
                messageTemplate = "The deletion process was started by the owner. It was automatically approved.";
                break;
            case "StartedBySupport":
                messageTemplate = "The deletion process was started by support. It is now waiting for approval.";
                break;
            case "Approved":
                messageTemplate = "The deletion process was approved.";
                break;
            case "Rejected":
                messageTemplate = "The deletion process was rejected.";
                break;
            case "CancelledByOwner":
                messageTemplate = "The deletion process was cancelled by the owner of the identity.";
                break;
            case "CancelledBySupport":
                messageTemplate = "The deletion process was cancelled by a support employee.";
                break;
            case "CancelledAutomatically":
                messageTemplate = "The deletion process was cancelled automatically, because it wasn't approved by the owner within the approval period.";
                break;
            case "ApprovalReminder1Sent":
                messageTemplate = "The first approval reminder notification has been sent.";
                break;
            case "ApprovalReminder2Sent":
                messageTemplate = "The second approval reminder notification has been sent.";
                break;
            case "ApprovalReminder3Sent":
                messageTemplate = "The third approval reminder notification has been sent.";
                break;
            case "GracePeriodReminder1Sent":
                messageTemplate = "The first grace period reminder notification has been sent.";
                break;
            case "GracePeriodReminder2Sent":
                messageTemplate = "The second grace period reminder notification has been sent.";
                break;
            case "GracePeriodReminder3Sent":
                messageTemplate = "The third grace period reminder notification has been sent.";
                break;
            case "DataDeleted":
                const messages: string[] = [];
                Object.keys(additionalData).forEach((key) => {
                    const message = `All ${additionalData[key]} have been deleted.`;
                    messages.push(message);
                });
                messageTemplate = messages.join("\n");
                break;
            default:
                messageTemplate = "Unknown message key.";
                break;
        }

        return messageTemplate;
    }
}
