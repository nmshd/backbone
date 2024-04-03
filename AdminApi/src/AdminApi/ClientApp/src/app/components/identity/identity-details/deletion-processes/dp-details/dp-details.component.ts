import { Component } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Router } from "@angular/router";
import { DeletionProcess, DeletionProcessAuditLog, IdentityService } from "src/app/services/identity-service/identity.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { CancelDeletionProcessDialogComponent } from "./cancel-dp-dialog/cancel-dp-dialog.component";

@Component({
    selector: "app-deletion-process-details",
    templateUrl: "./dp-details.component.html",
    styleUrl: "./dp-details.component.css"
})
export class DeletionProcessDetailsComponent {
    public identityDeletionProcessID: string;
    public identityAddress: string;

    public header: string;
    public headerDeletionProcessAuditLog: string;
    public headerDeletionProcessAuditLogDescription: string;

    public loading: boolean;
    public deletionProcessesAuditLogTableDisplayedColumns: string[];

    public identityDeletionProcess?: DeletionProcess;
    public identityDeletionProcessAuditLogs?: DeletionProcessAuditLog[];

    public constructor(
        private readonly identityService: IdentityService,
        private readonly snackBar: MatSnackBar,
        private readonly activatedRoute: ActivatedRoute,
        private readonly dialog: MatDialog,
        private readonly router: Router
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
            this.identityDeletionProcessID = params["id"];
        });
        this.loadDeletionProcess();
    }

    private loadDeletionProcess(): void {
        this.loading = true;
        this.identityService.getDeletionProcessOfIdentityById(this.identityAddress, this.identityDeletionProcessID).subscribe({
            next: (data: HttpResponseEnvelope<DeletionProcess>) => {
                this.identityDeletionProcess = data.result;
                this.identityDeletionProcessAuditLogs = this.identityDeletionProcess.auditLog;
            },
            complete: () => {
                this.loading = false;
            },
            error: (err: any) => {
                this.loading = false;
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public styleStatus(status: string): string {
        if (status === "WaitingForApproval") return "Waiting for Approval";
        return status;
    }

    public openCancelDeletionProcessDialog(): void {
        const dialogRef = this.dialog.open(CancelDeletionProcessDialogComponent, {
            maxWidth: "100%"
        });

        dialogRef.afterClosed().subscribe((result) => {
            if (result) {
                this.cancelDeletionProcess();
            }
        });
    }

    public cancelDeletionProcess(): void {
        this.loading = true;
        this.identityService.cancelDeletionProcessAsSupport(this.identityAddress, this.identityDeletionProcessID).subscribe({
            next: () => {
                this.snackBar.open("Identity updated successfully. Reloading...", "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            },
            complete: async () => {
                await this.router.navigate(["/identities", this.identityAddress]);
            },
            error: (err: any) => {
                this.loading = false;
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }
}
