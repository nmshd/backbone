import { Component, ViewChild } from "@angular/core";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { SelectionModel } from "@angular/cdk/collections";
import { Router } from "@angular/router";
import { MatDialog } from "@angular/material/dialog";
import { ClientDTO, ClientServiceService } from "src/app/services/client-service/client-service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { forkJoin, Observable } from "rxjs";
import { ConfirmationDialogComponent } from "../../shared/confirmation-dialog/confirmation-dialog.component";
import { ChangeSecretDialogComponent } from "../change-secret-dialog/change-secret-dialog.component";
import { Sort } from "@angular/material/sort";
@Component({
    selector: "app-client-list",
    templateUrl: "./client-list.component.html",
    styleUrls: ["./client-list.component.css"]
})
export class ClientListComponent {
    @ViewChild(MatPaginator) public paginator!: MatPaginator;
    public header: string;
    public headerDescription: string;
    public clients: ClientDTO[];
    public serverClients: ClientDTO[];
    public loading = false;
    public selection = new SelectionModel<ClientDTO>(true, []);
    public displayedColumns: string[] = ["select", "clientId", "displayName", "actions"];
    public clientIdFilter: string;
    public displayNameFilter: string;

    public constructor(
        private readonly router: Router,
        private readonly dialog: MatDialog,
        private readonly snackBar: MatSnackBar,
        private readonly clientService: ClientServiceService
    ) {
        this.header = "Clients";
        this.headerDescription = "A list of existing Clients";
        this.clients = [];
        this.serverClients = [];
        this.clientIdFilter = "";
        this.displayNameFilter = "";
        this.loading = true;
    }

    public ngOnInit(): void {
        this.getPagedData();
    }

    public getPagedData(): void {
        this.loading = true;
        this.selection = new SelectionModel<ClientDTO>(true, []);
        this.clientService.getClients().subscribe({
            next: (data: PagedHttpResponseEnvelope<ClientDTO>) => {
                this.clients = data.result;
                this.serverClients = data.result;
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

    public filterClients(): void {
        this.loading = true;
        this.clients = this.serverClients.filter((c) => c.clientId.indexOf(this.clientIdFilter) != -1 && c.clientId.indexOf(this.displayNameFilter) != -1);
        this.loading = false;
    }

    public clearFilter(filter: string): void {
        switch (filter) {
            case "clientId":
                this.clientIdFilter = "";
                break;
            case "displayName":
                this.displayNameFilter = "";
                break;
        }

        this.filterClients();
    }

    public onTableSort(sort: Sort): void {
        switch (sort.active) {
            case "clientId":
                this.clients = this.clients.sort((a, b) => (a.clientId > b.clientId ? 1 : b.clientId > a.clientId ? -1 : 0));
                break;
            case "displayName":
                this.clients = this.clients.sort((a, b) => (a.displayName > b.displayName ? 1 : b.displayName > a.displayName ? -1 : 0));
                break;
        }
    }

    public async addClient(): Promise<void> {
        await this.router.navigate(["/clients/create"]);
    }

    public openConfirmationDialog(): void {
        const confirmDialogHeader = this.selection.selected.length > 1 ? "Delete Clients" : "Delete Client";
        const confirmDialogMessage =
            this.selection.selected.length > 1 ? `Are you sure you want to delete the ${this.selection.selected.length} selected clients?` : "Are you sure you want to delete the selected client?";
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
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

    public deleteClient(): void {
        this.loading = true;
        const observableBatch: Observable<any>[] = [];
        this.selection.selected.forEach((item) => {
            observableBatch.push(this.clientService.deleteClient(item.clientId));
        });

        forkJoin(observableBatch).subscribe({
            next: (_: any) => {
                const successMessage: string = this.selection.selected.length > 1 ? `Successfully deleted ${this.selection.selected.length} clients.` : "Successfully deleted 1 client.";
                this.getPagedData();
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
        const numSelected = this.selection.selected.length;
        const numRows = this.clients.length;
        return numSelected === numRows;
    }

    public toggleAllRows(): void {
        if (this.isAllSelected()) {
            this.selection.clear();
            return;
        }

        this.selection.select(...this.clients);
    }

    public checkboxLabel(index?: number, row?: ClientDTO): string {
        if (!row || !index) {
            return `${this.isAllSelected() ? "deselect" : "select"} all`;
        }
        return `${this.selection.isSelected(row) ? "deselect" : "select"} row ${index + 1}`;
    }

    public openChangeSecretDialog(clientId: any): void {
        this.dialog.open(ChangeSecretDialogComponent, {
            data: { clientId: clientId },
            minWidth: "50%",
            maxWidth: "100%"
        });
    }
}
