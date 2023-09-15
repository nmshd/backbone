import { Component, ViewChild } from "@angular/core";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { IdentityOverview, IdentityService } from "src/app/services/identity-service/identity.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-identity-list",
    templateUrl: "./identity-list.component.html",
    styleUrls: ["./identity-list.component.css"]
})
export class IdentityListComponent {
    @ViewChild(MatPaginator) public paginator!: MatPaginator;

    public header: string;
    public headerDescription: string;

    public identities: IdentityOverview[];

    public totalRecords: number;
    public pageSize: number;
    public pageIndex: number;

    public loading = false;

    public displayedColumns: string[] = ["address", "tierName", "createdWithClient", "numberOfDevices", "createdAt", "lastLoginAt", "datawalletVersion", "identityVersion"];

    public constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly identityService: IdentityService
    ) {
        this.header = "Identities";
        this.headerDescription = "A list of existing Identities";

        this.identities = [];

        this.totalRecords = 0;
        this.pageSize = 10;
        this.pageIndex = 0;

        this.loading = true;
    }

    public ngOnInit(): void {
        this.getPagedData();
    }

    public getPagedData(): void {
        this.loading = true;
        this.identityService.getIdentities(this.pageIndex, this.pageSize).subscribe({
            next: (data: PagedHttpResponseEnvelope<IdentityOverview>) => {
                this.identities = data.result;
                if (data.pagination) {
                    this.totalRecords = data.pagination.totalRecords!;
                } else {
                    this.totalRecords = data.result.length;
                }
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

    public pageChangeEvent(event: PageEvent): void {
        this.pageIndex = event.pageIndex;
        this.pageSize = event.pageSize;
        this.getPagedData();
    }

    public async editIdentity(identityAddress: string): Promise<void> {
        await this.router.navigate([`/identities/${identityAddress}`]);
    }

    public async goToTier(tierId: string): Promise<void> {
        await this.router.navigate([`/tiers/${tierId}`]);
    }
}
