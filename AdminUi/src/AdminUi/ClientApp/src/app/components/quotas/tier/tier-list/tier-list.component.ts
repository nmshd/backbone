import { Component, ViewChild } from "@angular/core";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { Tier, TierService } from "src/app/services/tier-service/tier.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-tier-list",
    templateUrl: "./tier-list.component.html",
    styleUrls: ["./tier-list.component.css"]
})
export class TierListComponent {
    @ViewChild(MatPaginator) paginator!: MatPaginator;

    header: string;
    headerDescription: string;

    tiers: Tier[];

    totalRecords: number;
    pageSize: number;
    pageIndex: number;

    loading = false;

    displayedColumns: string[] = ["id", "name"];

    constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly tierService: TierService
    ) {
        this.header = "Tiers";
        this.headerDescription = "A list of existing Tiers";

        this.tiers = [];

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
        this.tierService.getTiers(this.pageIndex, this.pageSize).subscribe({
            next: (data: PagedHttpResponseEnvelope<Tier>) => {
                if (data) {
                    this.tiers = data.result;
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
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    pageChangeEvent(event: PageEvent) {
        this.pageIndex = event.pageIndex;
        this.pageSize = event.pageSize;
        this.getPagedData();
    }

    addTier() {
        this.router.navigate(["/tiers/create"]);
    }

    editTier(tier: Tier) {
        this.router.navigate([`/tiers/${  tier.id}`]);
    }
}
