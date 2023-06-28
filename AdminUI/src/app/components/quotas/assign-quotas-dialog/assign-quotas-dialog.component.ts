import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
    Metric,
    QuotasService,
} from 'src/app/services/quotas-service/quotas.service';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';

@Component({
    selector: 'app-assign-quotas-dialog',
    templateUrl: './assign-quotas-dialog.component.html',
    styleUrls: ['./assign-quotas-dialog.component.css'],
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
        private _snackBar: MatSnackBar,
        private quotasService: QuotasService,
        public dialogRef: MatDialogRef<AssignQuotasDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.header = 'Assign Quota';

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
        this.quotasService.getMetrics().subscribe({
            next: (data: HttpResponseEnvelope<Metric>) => {
                if (data && data.result) {
                    this.metrics = data.result;
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this._snackBar.open(err.message, 'Dismiss', {
                    panelClass: ['snack-bar'],
                });
            },
        });
    }

    getPeriods() {
        this.periods = this.quotasService.getPeriods();
    }

    assignQuota() {
        let quota: AssignQuotaData = {
            metricKey: this.metric.key,
            max: this.max,
            period: this.period,
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
