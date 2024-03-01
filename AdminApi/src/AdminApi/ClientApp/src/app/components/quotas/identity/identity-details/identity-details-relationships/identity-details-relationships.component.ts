import { Component, Input } from "@angular/core";
import { PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { Relationship, RelationshipService } from "src/app/services/relationship-service/relationship.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-identity-details-relationships",
    templateUrl: "./identity-details-relationships.component.html",
    styleUrls: ["./identity-details-relationships.component.css"]
})
export class IdentityDetailsRelationshipsComponent {
    @Input() public identityAddress?: string;

    public relationshipsTableDisplayedColumns: string[];
    public relationshipsTableData: Relationship[];

    public relationshipsTotalRecords: number;
    public relationshipsPageSize: number;
    public relationshipsPageIndex: number;

    public loading: boolean;
    public constructor(
        private readonly route: ActivatedRoute,
        private readonly snackBar: MatSnackBar,
        private readonly relationshipService: RelationshipService
    ) {
        this.relationshipsTableDisplayedColumns = ["peer", "requestedBy", "templateId", "status", "creationDate", "answeredAt", "createdByDevice", "answeredByDevice"];
        this.relationshipsTableData = [];
        this.relationshipsTotalRecords = 0;
        this.relationshipsPageSize = 10;
        this.relationshipsPageIndex = 0;
        this.loading = true;
    }

    public ngOnInit(): void {
        this.getRelationships();
    }

    public getRelationships(): void {
        this.loading = true;
        this.relationshipService.getRelationshipsByParticipantAddress(this.identityAddress!, this.relationshipsPageIndex, this.relationshipsPageSize).subscribe({
            next: (data: PagedHttpResponseEnvelope<Relationship>) => {
                this.relationshipsTableData = data.result;
                this.relationshipsTotalRecords = data.pagination?.totalRecords ? data.pagination.totalRecords : data.result.length;
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

    public relationshipsPageChangeEvent(event: PageEvent): void {
        this.relationshipsPageIndex = event.pageIndex;
        this.relationshipsPageSize = event.pageSize;
        this.getRelationships();
    }
}
