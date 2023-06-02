import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
    Quota,
    QuotasService,
} from 'src/app/services/quotas-service/quotas.service';
import { Metric } from 'src/app/services/quotas-service/quotas.service';

@Component({
    selector: 'app-assign-quotas-dialog',
    templateUrl: './assign-quotas-dialog.component.html',
    styleUrls: ['./assign-quotas-dialog.component.css'],
})
export class AssignQuotasDialogComponent {
    header: string;

    metric!: Metric;
    max: number;
    period!: string;

    metrics: Metric[];
    periods: string[];

    loading: boolean;

    constructor(
        private _snackBar: MatSnackBar,
        private quotasService: QuotasService,
        public dialogRef: MatDialogRef<AssignQuotasDialogComponent>
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
        this.quotasService.getMetrics().subscribe({
            next: (data: Metric[]) => {
                if (data) {
                    this.metrics = data;
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this._snackBar.open(err.message, 'Close');
            },
        });
    }

    getPeriods() {
        this.periods = this.quotasService.getPeriods();
    }

    assignQuota() {
        let quota: Quota = {
            metric: this.metric,
            max: this.max,
            period: this.period,
        };

        this.dialogRef.close(quota);
    }

    isValid(): boolean {
        return this.metric != null && this.period != null && this.max != null;
    }
}
