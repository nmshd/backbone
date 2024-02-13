import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { QuotasService } from "src/app/services/quotas-service/quotas.service";
import { Metric, MetricsService } from "src/app/services/metrics-service/metrics.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { Observable, catchError, finalize, tap } from "rxjs";

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
    }

    public ngOnInit(): void {
        this.getMetrics().subscribe(() => {
            if (this.data.assignQuotaData) {
                for (const metric of this.metrics) {
                    if (metric.key === this.data.assignQuotaData.metricKey) {
                        this.metric = metric;
                    }
                }
                this.max = this.data.assignQuotaData.max;
                this.period = this.data.assignQuotaData.period;
            }
        });
        this.getPeriods();
    }

    public getMetrics(): Observable<any> {
        this.loading = true;
        return this.metricsService.getMetrics().pipe(
            tap((data: HttpResponseEnvelope<Metric>) => {
                this.metrics = data.result;
            }),
            catchError(err => {
                this.loading = false;
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
                return err;
            }),
            finalize(() => {
                this.loading = false;
            })
        );
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

        this.dialogRef.close(quota);
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
