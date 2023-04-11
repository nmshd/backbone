import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import {
    Tier,
    TierService,
    TierDTO,
} from 'src/app/services/tier-service/tier.service';

@Component({
    selector: 'app-tier-list',
    templateUrl: './tier-list.component.html',
    styleUrls: ['./tier-list.component.css'],
    providers: [MessageService],
})
export class TierListComponent {
    @ViewChild('tiersTable') dt!: Table;
    header: string;

    tiers: Tier[];

    totalRecords: number;

    loading: boolean;

    constructor(
        private router: Router,
        private tierService: TierService,
        private messageService: MessageService
    ) {
        this.header = '';

        this.tiers = [];

        this.totalRecords = 0;
        this.loading = true;
    }

    ngOnInit() {
        this.header = 'Tiers';
    }

    loadTiers(event: LazyLoadEvent) {
        this.loading = true;

        setTimeout(() => {
            this.tierService
                .getTiers(event)
                .subscribe({
                    next: (data: TierDTO) => {
                        if (data) {
                            this.tiers = data.result;
                            if (data.pagination) {
                                this.totalRecords =
                                    data.pagination.totalRecords!;
                            } else {
                                this.totalRecords = data.result.length;
                            }
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

    clear() {
        this.dt.clear();
    }

    applyFilter($event: any, matchMode: string) {
        let value = ($event.target as HTMLInputElement)?.value;
        this.dt.filterGlobal(value, matchMode);
    }

    editTier(tier: Tier) {
        this.router.navigate([`/tiers`, tier.id]);
    }

    addTier() {
        this.router.navigate([`/tiers/create`]);
    }
}
