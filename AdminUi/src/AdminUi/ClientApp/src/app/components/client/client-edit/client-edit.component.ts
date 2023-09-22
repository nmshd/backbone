import { Component } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { Client, ClientDTO, UpdateClientRequest } from "src/app/services/client-service/client-service";
import { ClientServiceService } from "src/app/services/client-service/client-service";
import { Tier, TierService } from "src/app/services/tier-service/tier.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-client-edit",
    templateUrl: "./client-edit.component.html",
    styleUrls: ["./client-edit.component.css"]
})
export class ClientEditComponent {
    headerEdit: string;
    headerCreate: string;
    headerDescriptionEdit: string;
    headerDescriptionCreate: string;
    showPassword: boolean;
    clientId?: string;
    editMode: boolean;
    client: Client;
    tierList: Tier[];
    loading: boolean;
    disabled: boolean;
    displayClientSecretWarning: boolean;

    constructor(
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private clientService: ClientServiceService,
        private tierService: TierService
    ) {
        this.headerCreate = "Create Client";
        this.headerEdit = "Edit Client";
        this.headerDescriptionCreate = "Please fill the form below to create your Client";
        this.headerDescriptionEdit = "Perform your desired changes and save to edit your Client";
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

    ngOnInit(): void {
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

    initClient(): void {
        this.client = {
            clientId: "",
            displayName: "",
            clientSecret: ""
        } as Client;
    }

    getClient() {
        this.loading = true;
        this.clientService.getClientById(this.clientId!).subscribe({
            next: (data: HttpResponseEnvelope<ClientDTO>) => {
                if (data && data.result) {
                    this.client = {
                        clientId: data.result.clientId,
                        displayName: data.result.displayName!,
                        defaultTier: data.result.defaultTier
                    } as Client;
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

    getTiers(): void {
        this.loading = true;
        this.tierList = [];
        this.tierService.getTiers().subscribe({
            next: (data: PagedHttpResponseEnvelope<Tier>) => {
                if (data && data.result) {
                    this.tierList = data.result;
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

    createClient(): void {
        this.loading = true;

        let basicTier = this.tierList.find((tier) => tier.name == "Basic");
        if (basicTier) {
            this.client.defaultTier = basicTier.id;
        } else {
            this.snackBar.open("Basic Tier not found", "Dismiss", {
                verticalPosition: "top",
                horizontalPosition: "center"
            });

            return;
        }

        this.clientService.createClient(this.client).subscribe({
            next: (data: HttpResponseEnvelope<Client>) => {
                if (data && data.result) {
                    this.client = data.result;
                }
                this.displayClientSecretWarning = true;
                this.disabled = true;
                this.snackBar.open("Successfully added client.", "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
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

    updateClient(): void {
        this.loading = true;

        if (!this.client.defaultTier) {
            let basicTier = this.tierList.find((tier) => tier.name == "Basic");
            if (basicTier) {
                this.client.defaultTier = basicTier.id;
            } else {
                this.snackBar.open("Basic Tier not found", "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });

                return;
            }
        }

        let request = {
            defaultTier: this.client.defaultTier
        } as UpdateClientRequest;

        this.clientService.updateClient(this.client.clientId!, request).subscribe({
            next: (data: HttpResponseEnvelope<ClientDTO>) => {
                if (data && data.result) {
                    this.client = {
                        clientId: data.result.clientId,
                        displayName: data.result.displayName!,
                        defaultTier: data.result.defaultTier
                    } as Client;

                    this.snackBar.open("Successfully updated client.", "Dismiss", {
                        duration: 4000,
                        verticalPosition: "top",
                        horizontalPosition: "center"
                    });
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

    togglePasswordVisibility(): void {
        this.showPassword = !this.showPassword;
    }
}
