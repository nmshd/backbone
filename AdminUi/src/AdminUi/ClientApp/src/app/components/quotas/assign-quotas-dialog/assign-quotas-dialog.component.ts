import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { CreateQuotaForIdentityRequest, IdentityQuota, QuotasService } from "src/app/services/quotas-service/quotas.service";
import { Metric, MetricsService } from "src/app/services/metrics-service/metrics.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-assign-quotas-dialog",
    templateUrl: "./assign-quotas-dialog.component.html",
    styleUrls: ["./assign-quotas-dialog.component.css"]
})
export class AssignQuotasDialogComponent {
    public header: string;

    public metric: Metric | undefined;
    public max: number | null;
    public period: string | undefined;

    public metrics: any;
    public periods: string[];

    public loading: boolean;
    public errorMessage: string;

    public constructor(
        private readonly snackBar: MatSnackBar,
        private readonly quotasService: QuotasService,
        private readonly metricsService: MetricsService,
        public dialogRef: MatDialogRef<AssignQuotasDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.header = "Assign Quota";

        this.max = 0;

        this.metrics = [];
        this.periods = [];

        this.loading = true;
        this.errorMessage = "";
    }

    public ngOnInit(): void {
        this.getMetrics();
        this.getPeriods();
    }

    public getMetrics(): void {
        this.loading = true;
        this.metricsService.getMetrics().subscribe({
            next: (data: HttpResponseEnvelope<Metric>) => {
                this.metrics = data.result;
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

    public createQuota(quotaData: AssignQuotaData): void {
        this.loading = true;

        if (this.data.address) {
            this.createIdentityQuota(this.data.address, quotaData);
        } else if (this.data.tierId) {
            this.createTierQuota(this.data.tierId, quotaData);
        }
    }

    private createIdentityQuota(address: string, quotaData: AssignQuotaData) {
        const createQuotaRequest = {
            metricKey: quotaData.metricKey,
            max: quotaData.max,
            period: quotaData.period
        } as CreateQuotaForIdentityRequest;

        this.quotasService.createIdentityQuota(createQuotaRequest, address).subscribe({
            next: (_: HttpResponseEnvelope<IdentityQuota>) => {
                this.snackBar.open("Successfully assigned quota.", "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
                this.dialogRef.close(true);
            },
            complete: () => {
                this.loading = false;
            },
            error: (err: any) => {
                this.loading = false;
                this.errorMessage = err.error?.error?.message ?? err.message;
            }
        });
    }

    private createTierQuota(tierId: string, quotaData: AssignQuotaData) {
        this.quotasService.createTierQuota(quotaData, tierId).subscribe({
            next: () => {
                this.snackBar.open("Successfully assigned quota.", "Dismiss");
                this.dialogRef.close(true);
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.errorMessage = err.error?.error?.message ?? err.message;
            }
        });
    }

    public getPeriods(): void {
        this.periods = this.quotasService.getPeriods();
    }

    public assignQuota(): void {
        const quota: AssignQuotaData = {
            metricKey: this.metric!.key,
            max: this.max!,
            period: this.period!
        };

        this.createQuota(quota);
    }

    public isValid(): boolean {
        return this.metric !== undefined && this.period !== undefined && this.max !== null;
    }
}

export interface AssignQuotaData {
    metricKey: string;
    max: number;
    period: string;
}
