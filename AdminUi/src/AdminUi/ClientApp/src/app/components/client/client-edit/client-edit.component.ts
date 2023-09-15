import { Component } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { ClientServiceService, Client } from "src/app/services/client-service/client-service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-client-edit",
    templateUrl: "./client-edit.component.html",
    styleUrls: ["./client-edit.component.css"]
})
export class ClientEditComponent {
    public showPassword: boolean;
    public headerCreate: string;
    public headerDescription: string;
    public clientId?: string;
    public editMode: boolean;
    public client: Client;
    public loading: boolean;
    public disabled: boolean;
    public displayClientSecretWarning: boolean;

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly snackBar: MatSnackBar,
        private readonly clientService: ClientServiceService
    ) {
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
    }

    public ngOnInit(): void {
        this.route.params.subscribe((params) => {
            if (params["id"]) {
                this.clientId = params["id"];
            }
        });
        this.initClient();
    }

    public initClient(): void {
        this.client = {
            clientId: "",
            displayName: "",
            clientSecret: ""
        } as Client;

        this.loading = false;
    }

    public createClient(): void {
        this.loading = true;
        this.clientService.createClient(this.client).subscribe({
            next: (data: HttpResponseEnvelope<Client>) => {
                this.client = data.result;
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
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public togglePasswordVisibility(): void {
        this.showPassword = !this.showPassword;
    }
}
