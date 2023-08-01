import { Component } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Router } from "@angular/router";
import { QuotasService, TierQuota } from "src/app/services/quotas-service/quotas.service";
import { Tier, TierService } from "src/app/services/tier-service/tier.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { AssignQuotaData, AssignQuotasDialogComponent } from "../../assign-quotas-dialog/assign-quotas-dialog.component";
import { SelectionModel } from "@angular/cdk/collections";
import { ConfirmationDialogComponent } from "src/app/components/shared/confirmation-dialog/confirmation-dialog.component";
import { Observable, forkJoin } from "rxjs";
import { HttpErrorResponseWrapper } from "src/app/utils/http-error-response-wrapper";

@Component({
    selector: "app-tier-edit",
    templateUrl: "./tier-edit.component.html",
    styleUrls: ["./tier-edit.component.css"]
})
export class TierEditComponent {
    headerEdit: string;
    headerCreate: string;
    headerDescriptionEdit: string;
    headerDescriptionCreate: string;
    headerQuotas: string;
    headerQuotasDescription: string;
    selectionQuotas: SelectionModel<TierQuota>;
    quotasTableDisplayedColumns: string[];
    tierId?: string;
    disabled: boolean;
    editMode: boolean;
    tier: Tier;
    loading: boolean;

    constructor(private route: ActivatedRoute, private router: Router, private snackBar: MatSnackBar, private dialog: MatDialog, private tierService: TierService, private quotasService: QuotasService) {
        this.headerEdit = "Edit Tier";
        this.headerCreate = "Create Tier";
        this.headerDescriptionCreate = "Please fill the form below to create your Tier";
        this.headerDescriptionEdit = "Perform your desired changes and save to edit your Tier";
        this.headerQuotas = "Quotas";
        this.headerQuotasDescription = "View and assign quotas for this tier.";
        this.selectionQuotas = new SelectionModel<TierQuota>(true, []);
        this.quotasTableDisplayedColumns = ["select", "metricName", "max", "period"];
        this.editMode = false;
        this.loading = true;
        this.tier = {} as Tier;
        this.disabled = false;
        this.tier = {
            id: "",
            name: "",
            quotas: []
        } as Tier;
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            if (params["id"]) {
                this.tierId = params["id"];
                this.editMode = true;
            }
        });

        if (this.editMode) {
            this.getTier();
        } else {
            this.initTier();
        }
    }

    initTier() {
        this.tier = {
            name: ""
        } as Tier;

        this.loading = false;
    }

    getTier() {
        this.loading = true;
        this.selectionQuotas = new SelectionModel<TierQuota>(true, []);
        this.tierService.getTierById(this.tierId!).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tier = data.result;
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

    createTier() {
        this.loading = true;
        this.tierService.createTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tier = data.result;
                    this.tier.quotas = [];
                }
                this.snackBar.open("Successfully added tier.", "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
                this.tierId = data.result.id;
                this.editMode = true;
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

    updateTier() {
        this.loading = true;
        this.tierService.updateTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tier = data.result;
                    this.snackBar.open("Successfully updated tier.", "Dismiss", {
                        duration: 4000,
                        verticalPosition: "top",
                        horizontalPosition: "center"
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

    validateTier(): boolean {
        if (this.tier && this.tier.name && this.tier.name.length > 0) {
            return true;
        }
        return false;
    }

    openAssignQuotaDialog() {
        let dialogRef = this.dialog.open(AssignQuotasDialogComponent, {
            minWidth: "50%"
        });

        dialogRef.afterClosed().subscribe((result: AssignQuotaData) => {
            if (result) {
                this.createTierQuota(result);
            }
        });
    }

    createTierQuota(quota: AssignQuotaData) {
        this.loading = true;
        this.quotasService.createTierQuota(quota, this.tier.id).subscribe({
            next: (data: HttpResponseEnvelope<TierQuota>) => {
                if (data && data.result) {
                    this.snackBar.open("Successfully assigned quota.", "Dismiss");
                    this.tier.quotas.push(data.result);
                    this.tier.quotas = [...this.tier.quotas];
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

    openConfirmationDialogQuotaDeletion() {
        let confirmDialogHeader = this.selectionQuotas.selected.length > 1 ? "Delete Quotas" : "Delete Quota";
        let confirmDialogMessage =
            this.selectionQuotas.selected.length > 1
                ? `Are you sure you want to delete the ${this.selectionQuotas.selected.length} selected quotas?`
                : "Are you sure you want to delete the selected quota?";
        let dialogRef = this.dialog.open(ConfirmationDialogComponent, {
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

    openConfirmationDialogTierDeletion() {
        let confirmDialogHeader = "Delete Tier";
        let confirmDialogMessage = `Are you sure you want to delete the ${this.tier.name} tier?`;
        let dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            minWidth: "40%",
            disableClose: true,
            data: { header: confirmDialogHeader, message: confirmDialogMessage }
        });

        dialogRef.afterClosed().subscribe((result: boolean) => {
            if (result) {
                this.deleteTier();
            }
        });
    }

    deleteTier(): void {
        this.tierService.deleteTierById(this.tierId!).subscribe({
            next: (_: any) => {
                this.router.navigate(["/tiers"]);
            },
            error: (err: HttpErrorResponseWrapper) => {
                let errorMessage = err.error.error.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    deleteQuota(): void {
        this.loading = true;
        let observableBatch: Observable<any>[] = [];
        this.selectionQuotas.selected.forEach((item) => {
            observableBatch.push(this.quotasService.deleteTierQuota(item.id, this.tier.id));
        });

        forkJoin(observableBatch).subscribe({
            next: (_: any) => {
                let successMessage: string = this.selectionQuotas.selected.length > 1 ? `Successfully deleted ${this.selectionQuotas.selected.length} quotas.` : "Successfully deleted 1 quota.";
                this.getTier();
                this.snackBar.open(successMessage, "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            },
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

    isAllSelected() {
        const numSelected = this.selectionQuotas.selected.length;
        const numRows = this.tier.quotas.length;
        return numSelected === numRows;
    }

    toggleAllRowsQuotas() {
        if (this.isAllSelected()) {
            this.selectionQuotas.clear();
            return;
        }

        this.selectionQuotas.select(...this.tier.quotas);
    }

    checkboxLabelQuotas(index?: number, row?: TierQuota): string {
        if (!row || !index) {
            return `${this.isAllSelected() ? "deselect" : "select"} all`;
        }
        return `${this.selectionQuotas.isSelected(row) ? "deselect" : "select"} row ${index + 1}`;
    }
}
