import { Component } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { Identity, IdentityService } from "src/app/services/identity-service/identity.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { AssignQuotaData, AssignQuotasDialogComponent } from "../../assign-quotas-dialog/assign-quotas-dialog.component";
import { CreateQuotaForIdentityRequest, IdentityQuota, Metric, Quota, QuotasService } from "src/app/services/quotas-service/quotas.service";
import { MatDialog } from "@angular/material/dialog";

@Component({
    selector: "app-identity-edit",
    templateUrl: "./identity-edit.component.html",
    styleUrls: ["./identity-edit.component.css"]
})
export class IdentityEditComponent {
    header: string;
    headerDescription: string;
    headerQuotas: string;
    headerQuotasDescription: string;
    quotasTableDisplayedColumns: string[];
    quotasTableData: (Quota | MetricGroup)[];
    identityAddress?: string;
    disabled: boolean;
    identity: Identity;
    loading: boolean;

    constructor(private route: ActivatedRoute, private snackBar: MatSnackBar, private dialog: MatDialog, private identityService: IdentityService, private quotasService: QuotasService) {
        this.header = "Edit Identity";
        this.headerDescription = "Perform your desired changes for this Identity";
        this.headerQuotas = "Quotas";
        this.headerQuotasDescription = "View and assign quotas for this Identity.";
        this.quotasTableDisplayedColumns = ["metric", "source", "max", "period"];
        this.quotasTableData = [];
        this.loading = true;
        this.disabled = false;
        this.identity = {} as Identity;
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            if (params["address"]) {
                this.identityAddress = params["address"];
            }
        });

        this.getIdentity();
    }

    getIdentity() {
        this.loading = true;

        this.identityService.getIdentityByAddress(this.identityAddress!).subscribe({
            next: (data: HttpResponseEnvelope<Identity>) => {
                if (data && data.result) {
                    this.identity = data.result;
                    this.groupQuotasByMetricForTable();
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                let errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    groupQuotasByMetricForTable() {
        let quotas = [...this.identity.quotas];
        this.quotasTableData = [];

        quotas.sort((a, b) => a.metric.key.localeCompare(b.metric.key) || a.source.localeCompare(b.source));
        while (quotas.length > 0) {
            this.quotasTableData.push({
                metric: quotas[0].metric,
                isGroup: true
            } as MetricGroup);
            quotas = this.iterateQuotasByMetric(quotas, quotas[0].metric.key);
        }
    }

    iterateQuotasByMetric(quotas: Quota[], metricKey: string): Quota[] {
        if (quotas.length == 0) return [];

        if (quotas[0].metric.key == metricKey) {
            this.quotasTableData.push(quotas[0]);
            return this.iterateQuotasByMetric(quotas.slice(1), metricKey);
        }

        return quotas;
    }

    isGroup(index: any, item: any): boolean {
        return item.isGroup;
    }

    openAssignQuotaDialog() {
        let dialogRef = this.dialog.open(AssignQuotasDialogComponent, {
            minWidth: "50%"
        });

        dialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.createIdentityQuota(result);
            }
        });
    }

    createIdentityQuota(quotaData: AssignQuotaData) {
        this.loading = true;

        const createQuotaRequest = {
            metricKey: quotaData.metricKey,
            max: quotaData.max,
            period: quotaData.period
        } as CreateQuotaForIdentityRequest;

        this.quotasService.createIdentityQuota(createQuotaRequest, this.identity.address).subscribe({
            next: (data: HttpResponseEnvelope<IdentityQuota>) => {
                if (data && data.result) {
                    this.getIdentity();
                    this.snackBar.open("Successfully assigned quota.", "Dismiss", {
                        panelClass: ["snack-bar"]
                    });
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                let errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }
}

interface MetricGroup {
    metric: Metric;
    isGroup: boolean;
}
