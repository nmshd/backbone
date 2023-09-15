import { SelectionModel } from "@angular/cdk/collections";
import { Component } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { Observable, forkJoin } from "rxjs";
import { ConfirmationDialogComponent } from "src/app/components/shared/confirmation-dialog/confirmation-dialog.component";
import { Device, Identity, IdentityService } from "src/app/services/identity-service/identity.service";
import { CreateQuotaForIdentityRequest, IdentityQuota, Metric, Quota, QuotasService } from "src/app/services/quotas-service/quotas.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { AssignQuotaData, AssignQuotasDialogComponent } from "../../assign-quotas-dialog/assign-quotas-dialog.component";

@Component({
    selector: "app-identity-edit",
    templateUrl: "./identity-edit.component.html",
    styleUrls: ["./identity-edit.component.css"]
})
export class IdentityEditComponent {
    public header: string;
    public headerDescription: string;
    public headerQuotas: string;
    public headerQuotasDescription: string;
    public headerDevices: string;
    public headerDevicesDescription: string;
    public selectionQuotas: SelectionModel<IdentityQuota>;
    public quotasTableDisplayedColumns: string[];
    public quotasTableData: (Quota | MetricGroup)[];
    public devicesTableDisplayedColumns: string[];
    public devicesTableData: Device[];
    public identityAddress?: string;
    public disabled: boolean;
    public identity: Identity;
    public loading: boolean;

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly snackBar: MatSnackBar,
        private readonly dialog: MatDialog,
        private readonly identityService: IdentityService,
        private readonly quotasService: QuotasService
    ) {
        this.header = "Edit Identity";
        this.headerDescription = "Perform your desired changes for this Identity";
        this.headerQuotas = "Quotas";
        this.headerQuotasDescription = "View and assign quotas for this Identity.";
        this.headerDevices = "Devices";
        this.headerDevicesDescription = "View devices for this Identity.";
        this.quotasTableDisplayedColumns = ["select", "metric", "source", "max", "period"];
        this.quotasTableData = [];
        this.devicesTableDisplayedColumns = ["id", "username", "createdAt", "lastLogin", "createdByDevice"];
        this.devicesTableData = [];
        this.loading = true;
        this.disabled = false;
        this.identity = {} as Identity;
        this.selectionQuotas = new SelectionModel<IdentityQuota>(true, []);
    }

    public ngOnInit(): void {
        this.route.params.subscribe((params) => {
            if (params["address"]) {
                this.identityAddress = params["address"];
            }
        });

        this.getIdentity();
    }

    public getIdentity(): void {
        this.loading = true;
        this.selectionQuotas = new SelectionModel<IdentityQuota>(true, []);
        this.identityService.getIdentityByAddress(this.identityAddress!).subscribe({
            next: (data: HttpResponseEnvelope<Identity>) => {
                this.identity = data.result;
                this.groupQuotasByMetricForTable();
                this.devicesTableData = this.identity.devices;
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

    public groupQuotasByMetricForTable(): void {
        let quotas = [...this.identity.quotas];
        this.quotasTableData = [];

        quotas.sort((a, b) => a.metric.key.localeCompare(b.metric.key) || a.source.localeCompare(b.source));
        while (quotas.length > 0) {
            const metricGroup = {
                metric: quotas[0].metric,
                isGroup: true,
                tierDisabled: false
            } as MetricGroup;

            this.quotasTableData.push(metricGroup);
            quotas = this.iterateQuotasByMetricGroup(quotas, metricGroup);
        }
    }

    public iterateQuotasByMetricGroup(quotas: Quota[], metricGroup: MetricGroup): Quota[] {
        if (quotas.length === 0) return [];

        if (quotas[0].metric.key === metricGroup.metric.key) {
            this.quotasTableData.push(quotas[0]);
            if (quotas[0].source === "Individual") {
                metricGroup.tierDisabled = true;
                quotas[0].deleteable = true;
            }
            if (quotas[0].source === "Tier") {
                quotas[0].disabled = metricGroup.tierDisabled;
                quotas[0].deleteable = false;
            }
            return this.iterateQuotasByMetricGroup(quotas.slice(1), metricGroup);
        }

        return quotas;
    }

    public isGroup(index: any, item: any): boolean {
        return item.isGroup;
    }

    public openAssignQuotaDialog(): void {
        const dialogRef = this.dialog.open(AssignQuotasDialogComponent, {
            minWidth: "50%"
        });

        dialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.createIdentityQuota(result);
            }
        });
    }

    public createIdentityQuota(quotaData: AssignQuotaData): void {
        this.loading = true;

        const createQuotaRequest = {
            metricKey: quotaData.metricKey,
            max: quotaData.max,
            period: quotaData.period
        } as CreateQuotaForIdentityRequest;

        this.quotasService.createIdentityQuota(createQuotaRequest, this.identity.address).subscribe({
            next: () => {
                this.getIdentity();
                this.snackBar.open("Successfully assigned quota.", "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
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

    public openConfirmationDialogQuotaDeletion(): void {
        const confirmDialogHeader = this.selectionQuotas.selected.length > 1 ? "Delete Quotas" : "Delete Quota";
        const confirmDialogMessage =
            this.selectionQuotas.selected.length > 1
                ? `Are you sure you want to delete the ${this.selectionQuotas.selected.length} selected quotas?`
                : "Are you sure you want to delete the selected quota?";
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            minWidth: "40%",
            disableClose: true,
            data: { header: confirmDialogHeader, message: confirmDialogMessage }
        });

        dialogRef.afterClosed().subscribe((result: boolean) => {
            if (result) {
                this.deleteQuota();
            }
        });
    }

    public deleteQuota(): void {
        this.loading = true;
        const observableBatch: Observable<any>[] = [];
        this.selectionQuotas.selected.forEach((item) => {
            observableBatch.push(this.quotasService.deleteIdentityQuota(item.id, this.identity.address));
        });

        forkJoin(observableBatch).subscribe({
            next: (_: any) => {
                const successMessage: string = this.selectionQuotas.selected.length > 1 ? `Successfully deleted ${this.selectionQuotas.selected.length} quotas.` : "Successfully deleted 1 quota.";
                this.getIdentity();
                this.snackBar.open(successMessage, "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
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

    public isAllSelected(): boolean {
        if (this.loading) return false;
        const numSelected = this.selectionQuotas.selected.length;
        const numRows = this.identity.quotas ? this.identity.quotas.filter((i) => i.deleteable).length : 0;
        return numSelected === numRows;
    }

    public toggleAllRowsQuotas(): void {
        if (this.isAllSelected()) {
            this.selectionQuotas.clear();
            return;
        }
        this.selectionQuotas.select(...this.identity.quotas.filter((i) => i.deleteable));
    }

    public checkboxLabelQuotas(index?: number, row?: IdentityQuota): string {
        if (!row || !index) {
            return `${this.isAllSelected() ? "deselect" : "select"} all`;
        }
        return `${this.selectionQuotas.isSelected(row) ? "deselect" : "select"} row ${index + 1}`;
    }
}

interface MetricGroup {
    metric: Metric;
    isGroup: boolean;
    tierDisabled: boolean;
}
