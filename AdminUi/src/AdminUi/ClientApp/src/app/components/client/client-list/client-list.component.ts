import { SelectionModel } from "@angular/cdk/collections";
import { Component, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Sort } from "@angular/material/sort";
import { MatTable } from "@angular/material/table";
import { Router } from "@angular/router";
import { NGXLogger } from "ngx-logger";
import { Observable, forkJoin } from "rxjs";
import { ClientOverview, ClientOverviewFilter, ClientService } from "src/app/services/client-service/client-service";
import { TierOverview, TierService } from "src/app/services/tier-service/tier.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { ConfirmationDialogComponent } from "../../shared/confirmation-dialog/confirmation-dialog.component";
import { ChangeSecretDialogComponent } from "../change-secret-dialog/change-secret-dialog.component";
@Component({
    selector: "app-client-list",
    templateUrl: "./client-list.component.html",
    styleUrls: ["./client-list.component.css"]
})
export class ClientListComponent {
    @ViewChild(MatPaginator) public paginator!: MatPaginator;
    @ViewChild(MatTable) public table!: MatTable<any>;
    public header: string;
    public headerDescription: string;
    public clients: ClientOverview[];
    public serverClients: ClientOverview[];
    public loading = false;
    public filter: ClientOverviewFilter;
    public tiers: TierOverview[];
    public selection = new SelectionModel<ClientOverview>(true, []);
    public operators: string[] = ["=", "<", ">", ">=", "<="];
    public displayedColumns: string[] = ["select", "displayName", "defaultTier", "numberOfIdentities", "createdAt", "clientId", "actions"];

    public constructor(
        private readonly router: Router,
        private readonly dialog: MatDialog,
        private readonly snackBar: MatSnackBar,
        private readonly tierService: TierService,
        private readonly clientService: ClientService,
        private readonly logger: NGXLogger
    ) {
        this.header = "Clients";
        this.headerDescription = "A list of existing Clients";
        this.clients = [];
        this.tiers = [];
        this.serverClients = [];
        this.filter = { createdAt: { operator: "=" }, numberOfIdentities: { operator: "=" } };
        this.loading = true;
    }

    public ngOnInit(): void {
        this.getPagedData();
        this.getTiers();
    }

    public getTiers(): void {
        this.tierService.getTiers().subscribe({
            next: (data: PagedHttpResponseEnvelope<TierOverview>) => {
                this.tiers = data.result;
            },
            error: (err: any) => {
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public getPagedData(): void {
        this.loading = true;
        this.selection = new SelectionModel<ClientOverview>(true, []);
        this.clientService.getClients().subscribe({
            next: (data: PagedHttpResponseEnvelope<ClientOverview>) => {
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

        this.clients = this.serverClients;
        if (this.filter.clientId) this.clients = this.clients.filter((c) => c.clientId.toUpperCase().includes(this.filter.clientId!.toUpperCase()));
        if (this.filter.displayName) this.clients = this.clients.filter((c) => c.displayName.toUpperCase().includes(this.filter.displayName!.toUpperCase()));
        if (this.filter.tiers && this.filter.tiers.length > 0) this.clients = this.clients.filter((c) => this.filter.tiers!.includes(c.defaultTier.id));
        if (this.filter.numberOfIdentities.value !== undefined) {
            switch (this.filter.numberOfIdentities.operator) {
                case "=":
                    this.clients = this.clients.filter((c) => c.numberOfIdentities === this.filter.numberOfIdentities.value!);
                    break;
                case ">":
                    this.clients = this.clients.filter((c) => c.numberOfIdentities > this.filter.numberOfIdentities.value!);
                    break;
                case "<":
                    this.clients = this.clients.filter((c) => c.numberOfIdentities < this.filter.numberOfIdentities.value!);
                    break;
                case ">=":
                    this.clients = this.clients.filter((c) => c.numberOfIdentities >= this.filter.numberOfIdentities.value!);
                    break;
                case "<=":
                    this.clients = this.clients.filter((c) => c.numberOfIdentities <= this.filter.numberOfIdentities.value!);
                    break;
                default:
                    this.logger.error(`Invalid numberOfIdentities filter operator: ${this.filter.numberOfIdentities.operator}`);
                    break;
            }
        }
        if (this.filter.createdAt.value !== undefined) {
            switch (this.filter.createdAt.operator) {
                case "=":
                    this.clients = this.clients.filter((c) => new Date(c.createdAt).toISOString() === this.filter.createdAt.value!.toISOString());
                    break;
                case ">":
                    this.clients = this.clients.filter((c) => new Date(c.createdAt).toISOString() > this.filter.createdAt.value!.toISOString());
                    break;
                case "<":
                    this.clients = this.clients.filter((c) => new Date(c.createdAt).toISOString() < this.filter.createdAt.value!.toISOString());
                    break;
                case ">=":
                    this.clients = this.clients.filter((c) => new Date(c.createdAt).toISOString() >= this.filter.createdAt.value!.toISOString());
                    break;
                case "<=":
                    this.clients = this.clients.filter((c) => new Date(c.createdAt).toISOString() <= this.filter.createdAt.value!.toISOString());
                    break;
                default:
                    this.logger.error(`Invalid createdAt filter operator: ${this.filter.createdAt.operator}`);
                    break;
            }
        }
        this.loading = false;
    }

    public clearFilter(filter: string): void {
        switch (filter) {
            case "clientId":
                this.filter.clientId = "";
                break;
            case "displayName":
                this.filter.displayName = "";
                break;
            case "createdAt":
                this.filter.createdAt.value = undefined;
                break;
            default:
                this.logger.error(`Invalid filter: ${filter}`);
                break;
        }

        this.filterClients();
    }

    public onTableSort(sort: Sort): void {
        switch (sort.active) {
            case "clientId":
                if (sort.direction.toString() === "desc") {
                    this.clients.sort((a, b) => a.clientId.toLowerCase().localeCompare(b.clientId.toLowerCase()));
                } else {
                    this.clients.sort((a, b) => a.clientId.toLowerCase().localeCompare(b.clientId.toLowerCase())).reverse();
                }
                break;
            case "displayName":
                if (sort.direction.toString() === "desc") {
                    this.clients.sort((a, b) => a.displayName.toLowerCase().localeCompare(b.displayName.toLowerCase()));
                } else {
                    this.clients.sort((a, b) => a.displayName.toLowerCase().localeCompare(b.displayName.toLowerCase())).reverse();
                }
                break;
        }

        this.table.renderRows();
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

    public checkboxLabel(index?: number, row?: ClientOverview): string {
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

    public async editClient(clientId: string): Promise<void> {
        await this.router.navigate([`/clients/${clientId}`]);
    }

    public async goToTier(tierId: string): Promise<void> {
        await this.router.navigate([`/tiers/${tierId}`]);
    }
}
