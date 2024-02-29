import { Component } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { Client, ClientService, UpdateClientRequest } from "src/app/services/client-service/client-service";
import { TierOverview, TierService } from "src/app/services/tier-service/tier.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
// import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-client-edit",
    templateUrl: "./client-edit.component.html",
    styleUrls: ["./client-edit.component.css"]
})
export class ClientEditComponent {
    public headerEdit: string;

    public headerDescriptionEdit: string;

    public headerIdentities: string;
    public headerIdentitiesDescription: string;

    public showPassword: boolean;
    public clientId?: string;
    public editMode: boolean;
    public client: Client;
    public tierList: TierOverview[];
    public loading: boolean;
    public disabled: boolean;
    public displayClientSecretWarning: boolean;

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly snackBar: MatSnackBar,
        private readonly clientService: ClientService,
        private readonly tierService: TierService
    ) {
        this.headerEdit = "Edit Client";
        this.headerDescriptionEdit = "Perform your desired changes and save to edit your Client";
        this.headerIdentities = "Identities";
        this.headerIdentitiesDescription = "View and manage Identities created by this Client.";
        this.editMode = false;
        this.loading = true;
        this.disabled = false;
        this.displayClientSecretWarning = false;
        this.showPassword = false;
        this.client = {
            clientId: "",
            displayName: "",
            clientSecret: ""
        } as Client;
        this.tierList = [];
    }

    public ngOnInit(): void {
        this.route.params.subscribe((params) => {
            if (params["id"]) {
                this.clientId = params["id"];
                this.editMode = true;
            }
        });

        if (this.editMode) {
            this.getClient();
        } else {
            this.initClient();
        }

        this.getTiers();
    }

    public initClient(): void {
        this.client = {
            clientId: "",
            displayName: "",
            clientSecret: ""
        } as Client;
    }

    public getClient(): void {
        this.loading = true;
        this.clientService.getClientById(this.clientId!).subscribe({
            next: (data: HttpResponseEnvelope<Client>) => {
                this.client = {
                    clientId: data.result.clientId,
                    displayName: data.result.displayName,
                    defaultTier: data.result.defaultTier,
                    createdAt: data.result.createdAt,
                    maxIdentities: data.result.maxIdentities
                } as Client;
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

    public getTiers(): void {
        this.loading = true;
        this.tierList = [];
        this.tierService.getTiers().subscribe({
            next: () => {
                // data: PagedHttpResponseEnvelope<TierOverview>
                // this.tierList = data.result;
                this.tierList = this.tierService.createMockData().filter((tier) => tier.canBeUsedAsDefaultTier);
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

    public updateClient(): void {
        this.loading = true;

        const request = {
            defaultTier: this.client.defaultTier,
            maxIdentities: this.client.maxIdentities
        } as UpdateClientRequest;

        this.clientService.updateClient(this.client.clientId!, request).subscribe({
            next: (data: HttpResponseEnvelope<Client>) => {
                this.client = {
                    clientId: data.result.clientId,
                    displayName: data.result.displayName,
                    defaultTier: data.result.defaultTier,
                    createdAt: data.result.createdAt,
                    maxIdentities: data.result.maxIdentities
                } as Client;

                this.snackBar.open("Successfully updated client.", "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
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

    public canSaveClient(): boolean {
        if (!this.client.defaultTier) return false;
        if (this.client.maxIdentities !== undefined && this.client.maxIdentities < 0) return false;
        return true;
    }
}
