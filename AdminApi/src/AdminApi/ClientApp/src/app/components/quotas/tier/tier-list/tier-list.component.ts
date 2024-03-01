import { Component, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { Tier, TierOverview, TierService } from "src/app/services/tier-service/tier.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { CreateTierDialogComponent } from "../create-tier-dialog/create-tier-dialog.component";

@Component({
    selector: "app-tier-list",
    templateUrl: "./tier-list.component.html",
    styleUrls: ["./tier-list.component.css"]
})
export class TierListComponent {
    @ViewChild(MatPaginator) public paginator!: MatPaginator;

    public header: string;
    public headerDescription: string;

    public tiers: TierOverview[];

    public loading = false;

    public displayedColumns: string[] = ["name", "numberOfIdentities"];

    public constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly tierService: TierService,
        private readonly dialog: MatDialog
    ) {
        this.header = "Tiers";
        this.headerDescription = "A list of existing Tiers";

        this.tiers = [];

        this.loading = true;
    }

    public ngOnInit(): void {
        this.getTiers();
    }

    public getTiers(): void {
        this.loading = true;
        this.tierService.getTiers().subscribe({
            next: (data: PagedHttpResponseEnvelope<TierOverview>) => {
                this.tiers = data.result;
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

    public addTierDialog(): void {
        this.dialog
            .open(CreateTierDialogComponent, {
                width: "400px",
                maxHeight: "100%"
            })
            .afterClosed()
            .subscribe((shouldRedload: boolean) => {
                if (shouldRedload) this.getTiers();
            });
    }

    public async editTier(tier: Tier): Promise<void> {
        await this.router.navigate([`/tiers/${tier.id}`]);
    }
}
