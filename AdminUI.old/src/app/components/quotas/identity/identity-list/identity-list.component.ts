import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import {
    Identity,
    IdentityService,
} from 'src/app/services/identity-service/identity.service';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';

@Component({
    selector: 'app-identity-list-component',
    templateUrl: './identity-list.component.html',
    styleUrls: ['./identity-list.component.css'],
    providers: [MessageService],
})
export class IdentityListComponent {
    @ViewChild('identitiesTable') dt!: Table;
    header: string;

    identities: Identity[];

    totalRecords: number;

    loading: boolean;

    constructor(
        private router: Router,
        private identityService: IdentityService,
        private messageService: MessageService
    ) {
        this.header = '';

        this.identities = [];

        this.totalRecords = 0;
        this.loading = true;
    }

    ngOnInit() {
        this.header = 'Identities';
    }

    loadIdentities(event: LazyLoadEvent) {
        this.loading = true;
        setTimeout(() => {
            this.identityService
                .getIdentities(event)
                .subscribe({
                    next: (data: PagedHttpResponseEnvelope<Identity>) => {
                        if (data) {
                            this.identities = data.result;
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

    dateConvert(date: any) {
        return new Date(date).toLocaleDateString();
    }

    clear() {
        this.dt.clear();
    }

    applyFilter($event: any, matchMode: string) {
        let value = ($event.target as HTMLInputElement)?.value;
        this.dt.filterGlobal(value, matchMode);
    }

    editIdentity(identity: Identity) {
        this.router.navigate([`/identities`, identity.address]);
    }
}
