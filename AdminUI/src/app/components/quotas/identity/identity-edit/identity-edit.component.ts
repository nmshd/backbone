import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import {
    Identity,
    IdentityService,
} from 'src/app/services/identity-service/identity.service';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { AssignQuotasDialogComponent } from '../../assign-quotas-dialog/assign-quotas-dialog.component';
import {
    Quota,
    QuotasService,
} from 'src/app/services/quotas-service/quotas.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-identity-edit',
    templateUrl: './identity-edit.component.html',
    styleUrls: ['./identity-edit.component.css'],
})
export class IdentityEditComponent {
    header: string;

    identityAddress?: string;

    identity: Identity;

    loading: boolean;

    constructor(
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private dialog: MatDialog,
        private identityService: IdentityService,
        private quotasService: QuotasService
    ) {
        this.header = 'Edit Identity';
        this.loading = true;
        this.identity = {} as Identity;
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            if (params['address']) {
                this.identityAddress = params['address'];
            }
        });

        this.getIdentity();
    }

    getIdentity() {
        this.loading = true;

        this.identityService
            .getIdentityByAddress(this.identityAddress!)
            .subscribe({
                next: (data: HttpResponseEnvelope<Identity>) => {
                    if (data && data.result) {
                        this.identity = data.result;
                    }
                },
                complete: () => (this.loading = false),
                error: (err: any) => {
                    this.loading = false;
                    this.snackBar.open(err.message, 'Dismiss');
                },
            });
    }

    openAssignQuotaDialog() {
        let dialogRef = this.dialog.open(AssignQuotasDialogComponent);

        dialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.createIdentityQuota(result);
            }
        });
    }

    createIdentityQuota(quota: Quota) {
        this.loading = true;
        this.quotasService
            .createIdentityQuota(quota, this.identityAddress!)
            .subscribe({
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
