import { Component, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { Router } from '@angular/router';
import {
    Client,
    ClientDTO,
    ClientServiceService,
} from 'src/app/services/client-service/client-service';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
import { forkJoin, Observable } from 'rxjs';
@Component({
    selector: 'app-client-list',
    templateUrl: './client-list.component.html',
    styleUrls: ['./client-list.component.css']
})
export class ClientListComponent {

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    header: string;
    headerDescription: string;
    clients: ClientDTO[];
    totalRecords: number;
    pageSize: number;
    pageIndex: number;
    loading = false;
    selection = new SelectionModel<ClientDTO>(true, []);
    displayedColumns: string[] = [
        'select',
        'clientId',
        'displayName'
    ];

    constructor(
        private router: Router,
        private snackBar: MatSnackBar,
        private clientService: ClientServiceService
    ) {
        this.header = 'Clients';
        this.headerDescription = 'A list of existing Clients';
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
        this.clientService
            .getClients(this.pageIndex, this.pageSize)
            .subscribe({
                next: (data: PagedHttpResponseEnvelope<ClientDTO>) => {
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
                    let errorMessage = (err.error && err.error.error && err.error.error.message) ? err.error.error.message : err.message;
                    this.snackBar.open(errorMessage, 'Dismiss', {
                        verticalPosition: 'top',
                        horizontalPosition: 'center'
                    });
                },
            });
    }

    pageChangeEvent(event: PageEvent): void {
        this.pageIndex = event.pageIndex;
        this.pageSize = event.pageSize;
        this.getPagedData();
    }

    dateConvert(date: any): string {
        return new Date(date).toLocaleDateString();
    }

    addClient(): void {
        this.router.navigate([`/clients/create`]);
    }

    deleteClient(): void {
        this.loading = true;
        let observableBatch: Observable<any>[] = [];
        this.selection.selected.forEach(item => {
            observableBatch.push(this.clientService.deleteClient(item.clientId));
        });

        forkJoin(observableBatch)
            .subscribe({
                next: (data: any) => {
                    this.getPagedData();
                    this.snackBar.open('Successfully deleted clients.', 'Dismiss', {
                        duration: 4000,
                        verticalPosition: 'top',
                        horizontalPosition: 'center'
                    });
                },
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

    isAllSelected() {
        const numSelected = this.selection.selected.length;
        const numRows = this.clients.length;
        return numSelected === numRows;
    }

    toggleAllRows() {
        if (this.isAllSelected()) {
            this.selection.clear();
            return;
        }

        this.selection.select(...this.clients);
    }

    checkboxLabel(index?: number, row?: ClientDTO): string {
        if (!row || !index) {
            return `${this.isAllSelected() ? 'deselect' : 'select'} all`;
        }
        return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${index + 1}`;
    }
}
