import { SelectionModel } from "@angular/cdk/collections";
import { Component } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { Client } from "src/app/services/client-service/client-service";
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
    showPassword: boolean;
    headerCreate: string;
    headerDescription: string;
    clientId?: string;
    editMode: boolean;
    client: Client;
    tierList: Tier[];
    loading: boolean;
    disabled: boolean;
    displayClientSecretWarning: boolean;

    constructor(private route: ActivatedRoute, private snackBar: MatSnackBar, private clientService: ClientServiceService, private tierService: TierService) {
        this.headerCreate = "Create Client";
        this.headerDescription = "Please fill the form below to create your Client";
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
            }
        });
        this.initClient();
        this.getTiers();
    }

    initClient(): void {
        this.client = {
            clientId: "",
            displayName: "",
            clientSecret: ""
        } as Client;
    }

    getTiers(): void {
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

        this.loading = false;
    }

    createClient(): void {
        this.loading = true;
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

    togglePasswordVisibility(): void {
        this.showPassword = !this.showPassword;
    }
}
