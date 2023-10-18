import { Component, ElementRef, ViewChild } from "@angular/core";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { NGXLogger } from "ngx-logger";
import { debounceTime, distinctUntilChanged, filter, fromEvent, tap } from "rxjs";
import { ClientOverview, ClientServiceService } from "src/app/services/client-service/client-service";
import { IdentityOverview, IdentityOverviewFilter, IdentityService } from "src/app/services/identity-service/identity.service";
import { TierOverview, TierService } from "src/app/services/tier-service/tier.service";
import { ODataResponse } from "src/app/utils/odata-response";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-identity-list",
    templateUrl: "./identity-list.component.html",
    styleUrls: ["./identity-list.component.css"]
})
export class IdentityListComponent {
    @ViewChild(MatPaginator) public paginator!: MatPaginator;
    @ViewChild("addressFilter", { static: false }) public set addressFilter(addressFilterInput: ElementRef | undefined) {
        this.debounceFilter(addressFilterInput, "address");
    }
    @ViewChild("numberOfDevicesFilter", { static: false }) public set numberOfDevicesFilter(numberOfDevicesFilterInput: ElementRef | undefined) {
        this.debounceFilter(numberOfDevicesFilterInput, "numberOfDevices");
    }
    @ViewChild("datawalletVersionFilter", { static: false }) public set datawalletVersionFilter(datawalletVersionFilterInput: ElementRef | undefined) {
        this.debounceFilter(datawalletVersionFilterInput, "datawalletVersion");
    }
    @ViewChild("identityVersionFilter", { static: false }) public set identityVersionFilter(identityVersionFilterInput: ElementRef | undefined) {
        this.debounceFilter(identityVersionFilterInput, "identityVersion");
    }

    public header: string;
    public headerDescription: string;

    public identities: IdentityOverview[];

    public totalRecords: number;
    public pageSize: number;
    public pageIndex: number;

    public loading = false;

    public displayedColumns: string[] = ["address", "tierName", "createdWithClient", "numberOfDevices", "createdAt", "lastLoginAt", "datawalletVersion", "identityVersion"];
    public displayedColumnFilters: string[] = [
        "address-filter",
        "tier-filter",
        "client-filter",
        "number-of-devices-filter",
        "created-at-filter",
        "last-login-filter",
        "datawallet-version-filter",
        "identity-version-filter"
    ];
    public operators: string[] = ["=", "<", ">", ">=", "<="];

    public filter: IdentityOverviewFilter;
    public tiers: TierOverview[];
    public clients: ClientOverview[];

    public constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly identityService: IdentityService,
        private readonly tierService: TierService,
        private readonly clientService: ClientServiceService,
        private readonly logger: NGXLogger
    ) {
        this.header = "Identities";
        this.headerDescription = "A list of existing Identities";

        this.identities = [];

        this.totalRecords = 0;
        this.pageSize = 10;
        this.pageIndex = 0;

        this.filter = { createdAt: { operator: "=" }, numberOfDevices: { operator: "=" }, identityVersion: { operator: "=" }, lastLoginAt: { operator: "=" }, datawalletVersion: { operator: "=" } };
        this.tiers = [];
        this.clients = [];

        this.loading = true;
    }

    public ngOnInit(): void {
        this.getIdentities();
        this.getTiers();
        this.getClients();
    }

    private debounceFilter(filterElement: ElementRef | undefined, filterName: string): void {
        if (filterElement !== undefined) {
            fromEvent(filterElement.nativeElement, "keyup")
                .pipe(
                    filter(Boolean),
                    debounceTime(750),
                    distinctUntilChanged(),
                    tap((_) => {
                        this.onFilterChange(filterName);
                    })
                )
                .subscribe();
        }
    }

    public getTiers(): void {
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

    public getClients(): void {
        this.clientService.getClients().subscribe({
            next: (data: PagedHttpResponseEnvelope<ClientOverview>) => {
                this.clients = data.result;
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

    public getIdentities(): void {
        this.loading = true;
        this.identityService.getIdentities(this.filter, this.pageIndex, this.pageSize).subscribe({
            next: (data: ODataResponse<IdentityOverview[]>) => {
                this.identities = data.value;
                this.totalRecords = data.value.length;
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
        this.getIdentities();
    }

    public async editIdentity(identityAddress: string): Promise<void> {
        await this.router.navigate([`/identities/${identityAddress}`]);
    }

    public async goToTier(tierId: string): Promise<void> {
        await this.router.navigate([`/tiers/${tierId}`]);
    }

    public onFilterChange(filter: string): void {
        switch (filter) {
            case "address":
                if (this.filter.address!.length > 2) this.getIdentities();
                break;
            case "tiers":
            case "clients":
                this.getIdentities();
                break;
            case "numberOfDevices":
                if (
                    (this.filter.numberOfDevices.operator !== undefined && this.filter.numberOfDevices.value !== undefined) ||
                    (this.filter.numberOfDevices.operator !== undefined && this.filter.numberOfDevices.value === undefined)
                ) {
                    this.getIdentities();
                }
                break;
            case "createdAt":
                if (this.filter.createdAt.operator !== undefined && this.filter.createdAt.operator !== "" && this.filter.createdAt.value !== undefined) this.getIdentities();
                break;
            case "lastLoginAt":
                if (this.filter.lastLoginAt.operator !== undefined && this.filter.lastLoginAt.operator !== "" && this.filter.lastLoginAt.value !== undefined) this.getIdentities();
                break;
            case "datawalletVersion":
                if (
                    (this.filter.datawalletVersion.operator !== undefined && this.filter.datawalletVersion.value !== undefined) ||
                    (this.filter.datawalletVersion.operator !== undefined && this.filter.datawalletVersion.value === undefined)
                ) {
                    this.getIdentities();
                }
                break;
            case "identityVersion":
                if (
                    (this.filter.identityVersion.operator !== undefined && this.filter.identityVersion.value !== undefined) ||
                    (this.filter.identityVersion.operator !== undefined && this.filter.identityVersion.value === undefined)
                ) {
                    this.getIdentities();
                }
                break;
            default:
                this.logger.error(`OnFilterChange: Invalid filter name: ${filter}`);
                break;
        }
    }

    public clearFilter(filter: string): void {
        switch (filter) {
            case "address":
                this.filter.address = "";
                this.getIdentities();
                break;
            case "createdAt":
                this.filter.createdAt.value = undefined;
                if (this.filter.createdAt.operator !== undefined) this.getIdentities();
                break;
            case "lastLoginAt":
                this.filter.lastLoginAt.value = undefined;
                if (this.filter.lastLoginAt.operator !== undefined) this.getIdentities();
                break;
            default:
                this.logger.error(`ClearFilter: Invalid filter name: ${filter}`);
                break;
        }
    }
}
