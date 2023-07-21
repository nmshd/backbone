import { Component, ViewChild } from "@angular/core";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatTableDataSource } from "@angular/material/table";
import { Identity, IdentityService } from "src/app/services/identity-service/identity.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-identity-list",
    templateUrl: "./identity-list.component.html",
    styleUrls: ["./identity-list.component.css"]
})
export class IdentityListComponent {
    @ViewChild(MatPaginator) paginator!: MatPaginator;

    header: string;
    headerDescription: string;

    identities: Identity[];

    totalRecords: number;
    pageSize: number;
    pageIndex: number;

    loading = false;

    displayedColumns: string[] = ["address", "clientId", "publicKey", "createdAt"];

    constructor(private snackBar: MatSnackBar, private identityService: IdentityService) {
        this.header = "Identities";
        this.headerDescription = "A list of existing Identities";

        this.identities = [];

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
        this.identityService.getIdentities(this.pageIndex, this.pageSize).subscribe({
            next: (data: PagedHttpResponseEnvelope<Identity>) => {
                if (data) {
                    this.identities = data.result;
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

    pageChangeEvent(event: PageEvent) {
        this.pageIndex = event.pageIndex;
        this.pageSize = event.pageSize;
        this.getPagedData();
    }

    dateConvert(date: any) {
        return new Date(date).toLocaleDateString();
    }
}
