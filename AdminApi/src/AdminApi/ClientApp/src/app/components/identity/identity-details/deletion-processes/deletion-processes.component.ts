import { Component, Input } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { DeletionProcessOverview, IdentityService } from "src/app/services/identity-service/identity.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-deletion-processes",
    templateUrl: "./deletion-processes.component.html",
    styleUrl: "./deletion-processes.component.css"
})
export class DeletionProcessesComponent {
    @Input() public identityAddress?: string;

    public loading: boolean;
    public deletionProcessesTableDisplayedColumns: string[];

    public identityDeletionProcesses?: DeletionProcessOverview[];

    public constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly identityService: IdentityService
    ) {
        this.loading = false;
        this.deletionProcessesTableDisplayedColumns = ["id", "status", "createdAt", "approvalReminders", "approvedAt", "approvedByDevice", "gracePeriodReminders", "gracePeriodEndsAt"];
    }

    public ngOnInit(): void {
        this.getIdentitiesDeletionProcesses();
    }

    public getIdentitiesDeletionProcesses(): void {
        this.loading = true;
        this.identityService.getDeletionProcessesForIdentity(this.identityAddress!).subscribe({
            next: (data: HttpResponseEnvelope<DeletionProcessOverview[]>) => {
                this.identityDeletionProcesses = data.result;
            },
            complete: () => (this.loading = false),
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

    public async goToDeletionProcessDetails(id: string): Promise<void> {
        await this.router.navigate(["/deletion-process-details/", this.identityAddress, id]);
    }

    public isDatePassed(date: Date): boolean {
        return new Date(date) < new Date();
    }

    public isRowDisabled(deletionProcess: any): boolean {
        return deletionProcess.status === "Rejected" || deletionProcess.status === "Cancelled";
    }

    public styleDeletionProcessStatus(status: string): string {
        return status === "WaitingForApproval" ? "Waiting for Approval" : status;
    }
}
