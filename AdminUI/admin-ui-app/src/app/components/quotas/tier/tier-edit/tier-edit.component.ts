import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Tier, TierService } from 'src/app/services/tier-service/tier.service';

@Component({
    selector: 'app-tier-edit',
    templateUrl: './tier-edit.component.html',
    styleUrls: ['./tier-edit.component.css'],
    providers: [MessageService],
})
export class TierEditComponent {
    header: string;
    titleEdit: string;
    titleCreate: string;

    tierId?: string;
    editMode: boolean;

    tier: Tier;

    loading: boolean;
    disabled: boolean;

    constructor(
        private route: ActivatedRoute,
        private tierService: TierService,
        private messageService: MessageService
    ) {
        this.header = '';
        this.titleEdit = '';
        this.titleCreate = '';

        this.editMode = false;
        this.loading = true;
        this.disabled = false;
        this.tier = {};
    }

    ngOnInit() {
        this.header = 'Tiers';
        this.titleEdit = 'Edit Tier';
        this.titleCreate = 'Create Tier';

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

    getTier() {
        this.loading = true;
        setTimeout(() => {
            this.tierService
                .getTierById(this.tierId!)
                .subscribe({
                    next: (data: any) => {
                        if (data && data.result) {
                            this.tier = data.result;
                        }
                    },
                    error: (err: any) => {
                        this.messageService.add({
                            severity: 'error',
                            summary: err.status,
                            detail: err.message,
                            sticky: true,
                        });
                        this.disabled = true;
                    },
                })
                .add(() => (this.loading = false));
        }, 1000);
    }

    createTier() {
        this.loading = true;
        setTimeout(() => {
            this.tierService
                .createTier(this.tier)
                .subscribe({
                    next: (data: any) => {
                        if (data && data.result) {
                            this.tier = data.result;
                        }
                        this.messageService.add({
                            severity: 'success',
                            summary: 'Success',
                            detail: 'Successfully added tier.',
                            sticky: true,
                        });
                        this.tierId = data.id;
                        this.editMode = true;
                    },
                    error: (err: any) =>
                        this.messageService.add({
                            severity: 'error',
                            summary: err.status,
                            detail: err.message,
                            sticky: true,
                        }),
                })
                .add(() => (this.loading = false));
        }, 1000);
    }

    updateTier() {
        this.loading = true;
        setTimeout(() => {
            this.tierService
                .updateTier(this.tier)
                .subscribe({
                    next: (data: any) => {
                        if (data && data.result) {
                            this.tier = data.result;
                            this.messageService.add({
                                severity: 'success',
                                summary: 'Success',
                                detail: 'Successfully updated tier.',
                                sticky: true,
                            });
                        }
                    },
                    error: (err: any) =>
                        this.messageService.add({
                            severity: 'error',
                            summary: err.status,
                            detail: err.message,
                            sticky: true,
                        }),
                })
                .add(() => (this.loading = false));
        }, 1000);
    }

    validateTier(): boolean {
        if (this.tier && this.tier.name && this.tier.name.length > 0) {
            return true;
        }
        return false;
    }

    initTier() {
        this.tier = {
            name: '',
        } as Tier;

        this.loading = false;
    }
}
