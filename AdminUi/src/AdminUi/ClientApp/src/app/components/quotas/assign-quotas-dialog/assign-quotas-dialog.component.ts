import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { QuotasService } from "src/app/services/quotas-service/quotas.service";
import { Metric, MetricsService } from "src/app/services/metrics-service/metrics.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-assign-quotas-dialog",
    templateUrl: "./assign-quotas-dialog.component.html",
    styleUrls: ["./assign-quotas-dialog.component.css"]
})
export class AssignQuotasDialogComponent {
    header: string;

    metric!: any;
    max: number;
    period!: string;

    metrics: any;
    periods: string[];

    loading: boolean;

    constructor(
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
    }

    ngOnInit() {
        this.getMetrics();
        this.getPeriods();
    }

    getMetrics() {
        this.loading = true;
        this.metricsService.getMetrics().subscribe({
            next: (data: HttpResponseEnvelope<Metric>) => {
                if (data && data.result) {
                    this.metrics = data.result;
                }
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

    getPeriods() {
        this.periods = this.quotasService.getPeriods();
    }

    assignQuota() {
        const quota: AssignQuotaData = {
            metricKey: this.metric.key,
            max: this.max,
            period: this.period
        };

        this.dialogRef.close(quota);
    }

    isValid(): boolean {
        return this.metric != null && this.period != null && this.max != null;
    }
}

export interface AssignQuotaData {
    metricKey: string;
    max: number;
    period: string;
}
