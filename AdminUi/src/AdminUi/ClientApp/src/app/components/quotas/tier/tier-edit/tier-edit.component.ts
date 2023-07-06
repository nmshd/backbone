import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import {
    CreateQuotaForTierRequest,
    QuotasService,
    TierQuota,
} from 'src/app/services/quotas-service/quotas.service';
import { Tier, TierService } from 'src/app/services/tier-service/tier.service';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import {
    AssignQuotaData,
    AssignQuotasDialogComponent,
} from '../../assign-quotas-dialog/assign-quotas-dialog.component';

@Component({
    selector: 'app-tier-edit',
    templateUrl: './tier-edit.component.html',
    styleUrls: ['./tier-edit.component.css'],
})
export class TierEditComponent {

    headerEdit: string;
    headerCreate: string;
    headerDescriptionEdit: string;
    headerDescriptionCreate: string;
    tierId?: string;
    disabled: boolean;
    editMode: boolean;
    tier: Tier;
    loading: boolean;

    constructor(
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private dialog: MatDialog,
        private tierService: TierService,
        private quotasService: QuotasService
    ) {
        this.headerEdit = 'Edit Tier';
        this.headerCreate = 'Create Tier';
        this.headerDescriptionCreate = 'Please fill the form below to create your Tier';
        this.headerDescriptionEdit = 'Perform your desired changes and save to edit your Tier';
        this.editMode = false;
        this.loading = true;
        this.tier = {} as Tier;
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            if (params['id']) {
                this.tierId = params['id'];
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
            name: '',
        } as Tier;

        this.loading = false;
    }

    getTier() {
        this.loading = true;

        this.tierService.getTierById(this.tierId!).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tier = data.result;
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                let errorMessage = (err.error && err.error.error && err.error.error.message) ? err.error.error.message : err.message;
                this.snackBar.open(errorMessage, 'Dismiss', {
                    verticalPosition: 'top',
                    horizontalPosition: 'center'
                });
            },
        });
    }

    createTier() {
        this.loading = true;
        this.tierService.createTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tier = data.result;
                }
                this.snackBar.open('Successfully added tier.', 'Dismiss', {
                    duration: 4000,
                    verticalPosition: 'top',
                    horizontalPosition: 'center'
                });
                this.tierId = data.result.id;
                this.editMode = true;
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                let errorMessage = (err.error && err.error.error && err.error.error.message) ? err.error.error.message : err.message;
                this.snackBar.open(errorMessage, 'Dismiss', {
                    verticalPosition: 'top',
                    horizontalPosition: 'center'
                });
                this.disabled = true;
            },
        });
    }

    updateTier() {
        this.loading = true;
        this.tierService.updateTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tier = data.result;
                    this.snackBar.open('Successfully updated tier.', 'Dismiss', {
                        duration: 4000,
                        verticalPosition: 'top',
                        horizontalPosition: 'center'
                    });
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                let errorMessage = (err.error && err.error.error && err.error.error.message) ? err.error.error.message : err.message;
                this.snackBar.open(errorMessage, 'Dismiss', {
                    verticalPosition: 'top',
                    horizontalPosition: 'center'
                });
            },
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
            minWidth: '50%'
        });

        dialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.createTierQuota(result);
            }
        });
    }

    createTierQuota(quotaData: AssignQuotaData) {
        this.loading = true;

        const createQuotaRequest = {
            metricKey: quotaData.metricKey,
            max: quotaData.max,
            period: quotaData.period,
        } as CreateQuotaForTierRequest;

        this.quotasService
            .createTierQuota(createQuotaRequest, this.tier.id)
            .subscribe({
                next: (data: HttpResponseEnvelope<TierQuota>) => {
                    if (data && data.result) {
                        this.snackBar.open(
                            'Successfully assigned quota.',
                            'Dismiss',
                            {
                                panelClass: ['snack-bar'],
                            }
                        );
                    }
                },
                complete: () => (this.loading = false),
                error: (err: any) => {
                    this.loading = false;
                    this.snackBar.open(err.message, 'Dismiss', {
                        panelClass: ['snack-bar'],
                    });
                },
            });
    }
}
