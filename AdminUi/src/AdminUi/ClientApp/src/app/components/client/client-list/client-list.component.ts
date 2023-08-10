import { Component, ViewChild } from "@angular/core";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatTableDataSource } from "@angular/material/table";
import { SelectionModel } from "@angular/cdk/collections";
import { Router } from "@angular/router";
import { MatDialog } from "@angular/material/dialog";
import { ClientDTO, ClientServiceService } from "src/app/services/client-service/client-service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { forkJoin, Observable } from "rxjs";
import { ConfirmationDialogComponent } from "../../shared/confirmation-dialog/confirmation-dialog.component";
import { ChangeSecretDialogComponent } from "../change-secret-dialog/change-secret-dialog.component";
@Component({
    selector: "app-client-list",
    templateUrl: "./client-list.component.html",
    styleUrls: ["./client-list.component.css"]
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
    displayedColumns: string[] = ["select", "clientId", "displayName", "actions"];

    constructor(
        private router: Router,
        private dialog: MatDialog,
        private snackBar: MatSnackBar,
        private clientService: ClientServiceService
    ) {
        this.header = "Clients";
        this.headerDescription = "A list of existing Clients";
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
        this.selection = new SelectionModel<ClientDTO>(true, []);
        this.clientService.getClients(this.pageIndex, this.pageSize).subscribe({
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
                let errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
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

    openConfirmationDialog() {
        let confirmDialogHeader = this.selection.selected.length > 1 ? "Delete Clients" : "Delete Client";
        let confirmDialogMessage =
            this.selection.selected.length > 1 ? `Are you sure you want to delete the ${this.selection.selected.length} selected clients?` : "Are you sure you want to delete the selected client?";
        let dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            minWidth: "40%",
            disableClose: true,
            data: { header: confirmDialogHeader, message: confirmDialogMessage }
        });

        dialogRef.afterClosed().subscribe((result: boolean) => {
            if (result) {
                this.deleteClient();
            }
        });
    }

    deleteClient(): void {
        this.loading = true;
        let observableBatch: Observable<any>[] = [];
        this.selection.selected.forEach((item) => {
            observableBatch.push(this.clientService.deleteClient(item.clientId));
        });

        forkJoin(observableBatch).subscribe({
            next: (_: any) => {
                let successMessage: string = this.selection.selected.length > 1 ? `Successfully deleted ${this.selection.selected.length} clients.` : "Successfully deleted 1 client.";
                this.getPagedData();
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
            return `${this.isAllSelected() ? "deselect" : "select"} all`;
        }
        return `${this.selection.isSelected(row) ? "deselect" : "select"} row ${index + 1}`;
    }

    openChangeSecretDialog(clientId: any) {
        let dialogRef = this.dialog.open(ChangeSecretDialogComponent, {
            data: { clientId: clientId },
            minWidth: "50%",
            maxWidth: "100%"
        });
    }
}
