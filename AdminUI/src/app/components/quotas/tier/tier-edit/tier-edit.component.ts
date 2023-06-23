import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import {
    Quota,
    QuotasService,
} from 'src/app/services/quotas-service/quotas.service';
import { Tier, TierService } from 'src/app/services/tier-service/tier.service';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { AssignQuotasDialogComponent } from '../../assign-quotas-dialog/assign-quotas-dialog.component';

@Component({
    selector: 'app-tier-edit',
    templateUrl: './tier-edit.component.html',
    styleUrls: ['./tier-edit.component.css'],
})
export class TierEditComponent {
    headerEdit: string;
    headerCreate: string;

    tierId?: string;

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
                this.snackBar.open(err.message, 'Dismiss');
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
                this.snackBar.open('Successfully added tier.', 'Dismiss');
                this.tierId = data.result.id;
                this.editMode = true;
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.snackBar.open(err.message, 'Dismiss');
            },
        });
    }

    updateTier() {
        this.loading = true;
        this.tierService.updateTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tier = data.result;
                    this.snackBar.open('Successfully updated tier.', 'Dismiss');
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.snackBar.open(err.message, 'Dismiss');
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
        let dialogRef = this.dialog.open(AssignQuotasDialogComponent);

        dialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.createTierQuota(result);
            }
        });
    }

    createTierQuota(quota: Quota) {
        this.loading = true;
        this.quotasService.createTierQuota(quota, this.tier.id).subscribe({
            next: (data: HttpResponseEnvelope<Quota>) => {
                if (data && data.result) {
                    this.snackBar.open(
                        'Successfully assigned quota.',
                        'Dismiss'
                    );
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.snackBar.open(err.message, 'Dismiss');
            },
        });
    }
}
