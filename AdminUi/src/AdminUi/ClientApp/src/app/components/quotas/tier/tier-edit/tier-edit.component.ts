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
    public headerEdit: string;
    public headerCreate: string;
    public headerDescriptionEdit: string;
    public headerDescriptionCreate: string;
    public headerQuotas: string;
    public headerQuotasDescription: string;
    public selectionQuotas: SelectionModel<TierQuota>;
    public quotasTableDisplayedColumns: string[];
    public tierId?: string;
    public editMode: boolean;
    public tier: Tier;
    public loading: boolean;

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly dialog: MatDialog,
        private readonly tierService: TierService,
        private readonly quotasService: QuotasService
    ) {
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
        this.tier = {
            id: "",
            name: "",
            quotas: [],
            isDeletable: false,
            isReadOnly: false
        } as Tier;
    }

    public ngOnInit(): void {
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

    public initTier(): void {
        this.tier = {
            name: ""
        } as Tier;

        this.loading = false;
    }

    public getTier(): void {
        this.loading = true;
        this.selectionQuotas = new SelectionModel<TierQuota>(true, []);
        this.tierService.getTierById(this.tierId!).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                this.tier = data.result;
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

    public createTier(): void {
        this.loading = true;
        this.tierService.createTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                this.tier = {
                    id: data.result.id,
                    name: data.result.name,
                    quotas: [],
                    numberOfIdentities: 0,
                    isDeletable: true,
                    isReadOnly: false
                } as Tier;

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
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public updateTier(): void {
        this.loading = true;
        this.tierService.updateTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                this.tier = data.result;
                this.snackBar.open("Successfully updated tier.", "Dismiss", {
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

    public validateTier(): boolean {
        if (this.tier.name.length > 0) {
            return true;
        }
        return false;
    }

    public openAssignQuotaDialog(): void {
        const dialogRef = this.dialog.open(AssignQuotasDialogComponent, {
            minWidth: "50%"
        });

        dialogRef.afterClosed().subscribe((result: AssignQuotaData | undefined) => {
            if (result) {
                this.createTierQuota(result);
            }
        });
    }

    public createTierQuota(quota: AssignQuotaData): void {
        this.loading = true;
        this.quotasService.createTierQuota(quota, this.tier.id).subscribe({
            next: (data: HttpResponseEnvelope<TierQuota>) => {
                this.snackBar.open("Successfully assigned quota.", "Dismiss");
                this.tier.quotas.push(data.result);
                this.tier.quotas = [...this.tier.quotas];
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

    public openConfirmationDialogTierDeletion(): void {
        const confirmDialogHeader = "Delete Tier";
        const confirmDialogMessage = `Are you sure you want to delete the ${this.tier.name} tier?`;
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
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

    public deleteTier(): void {
        this.tierService.deleteTierById(this.tierId!).subscribe({
            next: async (_) => {
                await this.router.navigate(["/tiers"]);
            },
            error: (err: HttpErrorResponseWrapper) => {
                const errorMessage = err.error.error.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public deleteQuota(): void {
        this.loading = true;
        const observableBatch: Observable<any>[] = [];
        this.selectionQuotas.selected.forEach((item) => {
            observableBatch.push(this.quotasService.deleteTierQuota(item.id, this.tier.id));
        });

        forkJoin(observableBatch).subscribe({
            next: (_: any) => {
                const successMessage: string = this.selectionQuotas.selected.length > 1 ? `Successfully deleted ${this.selectionQuotas.selected.length} quotas.` : "Successfully deleted 1 quota.";
                this.getTier();
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
        const numSelected = this.selectionQuotas.selected.length;
        const numRows = this.tier.quotas.length;
        return numSelected === numRows;
    }

    public toggleAllRowsQuotas(): void {
        if (this.isAllSelected()) {
            this.selectionQuotas.clear();
            return;
        }

        this.selectionQuotas.select(...this.tier.quotas);
    }

    public checkboxLabelQuotas(index?: number, row?: TierQuota): string {
        if (!row || !index) {
            return `${this.isAllSelected() ? "deselect" : "select"} all`;
        }
        return `${this.selectionQuotas.isSelected(row) ? "deselect" : "select"} row ${index + 1}`;
    }

    public isNameInputDisabled(): boolean {
        return this.editMode || this.tier.isReadOnly;
    }

    public isQuotaDeletionDisabled(): boolean {
        return this.selectionQuotas.selected.length === 0 || this.tier.isReadOnly;
    }

    public isQuotaAssignmentDisabled(): boolean {
        return this.tier.id === "" || this.tier.isReadOnly;
    }
}
