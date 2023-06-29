import { Component, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import {
    Client,
    ClientServiceService,
} from 'src/app/services/client-service/client-service';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
@Component({
    selector: 'app-client-list',
    templateUrl: './client-list.component.html',
    styleUrls: ['./client-list.component.css'],
})
export class ClientListComponent {
    @ViewChild(MatPaginator) paginator!: MatPaginator;

    header = 'Clients';

    clients: Client[];

    totalRecords: number;
    pageSize: number;
    pageIndex: number;

    loading = false;

    displayedColumns: string[] = ['clientId', 'displayName'];

    constructor(
        private _snackBar: MatSnackBar,
        private clientService: ClientServiceService
    ) {
        this.header = 'Clients';

        this.clients = [];

        this.totalRecords = 0;
        this.pageSize = 10;
        this.pageIndex = 0;

        this.loading = true;
    }

    ngOnInit() {
        this.getPagedData();
    }

    getPagedData() {
        this.loading = true;
        this.clientService.getClients(this.pageIndex, this.pageSize).subscribe({
            next: (data: PagedHttpResponseEnvelope<Client>) => {
                if (data) {
                    this.clients = data.result;
                    if (data.pagination) {
                        this.totalRecords = data.pagination.totalRecords!;
                    } else {
                        this.totalRecords = data.result.length;
                    }
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this._snackBar.open(err.message, 'Dismiss', {
                    panelClass: ['snack-bar'],
                });
            },
        });
    }

    pageChangeEvent(event: PageEvent) {
        this.pageIndex = event.pageIndex;
        this.pageSize = event.pageSize;
        this.getPagedData();
    }
}
