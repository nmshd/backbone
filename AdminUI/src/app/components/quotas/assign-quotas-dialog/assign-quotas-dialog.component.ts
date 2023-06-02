import { Component } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
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

    metric: Metric;
    max: number;
    range = new FormGroup({
        from: new FormControl<Date | null>(null),
        to: new FormControl<Date | null>(null),
    });
    period: string;

    metrics: Metric[];
    periods: string[];

    loading: boolean;

    constructor(
        private _snackBar: MatSnackBar,
        private quotasService: QuotasService,
        public dialogRef: MatDialogRef<AssignQuotasDialogComponent>
    ) {
        this.header = 'Assign Quota';

        this.metric = {};
        this.max = 0;
        this.period = '';

        this.metrics = [];
        this.periods = ['Monthly', 'Yearly'];

        this.loading = true;
    }

    ngOnInit() {
        this.getMetrics();
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

    assignQuota() {
        let quota: Quota = {
            metric: this.metric,
            max: this.max,
            validFrom: this.range.value.from!,
            validTo: this.range.value.to!,
            period: this.period,
        };

        this.dialogRef.close(quota);
    }

    isValid(): boolean {
        return (
            this.metric != null &&
            this.period != null &&
            this.range.value.from != null &&
            this.range.value.to != null
        );
    }
}
