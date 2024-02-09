import { Component } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Client, ClientService } from "src/app/services/client-service/client-service";
import { TierOverview, TierService } from "src/app/services/tier-service/tier.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

@Component({
    selector: "app-create-client-dialog",
    templateUrl: "./create-client-dialog.component.html",
    styleUrl: "./create-client-dialog.component.css"
})
export class CreateClientDialogComponent {
    public headerCreate: string;
    public headerDescriptionCreate: string;

    public disabled: boolean;
    public displayClientSecretWarning: boolean;
    public showPassword: boolean;

    public client: Client;
    public tierList: TierOverview[];
    public clientId?: string;

    public constructor(
        public readonly dialogRef: MatDialogRef<CreateClientDialogComponent>,
        private readonly clientService: ClientService,
        private readonly tierService: TierService,
        private readonly snackBar: MatSnackBar
    ) {
        this.headerCreate = "Create Client";
        this.headerDescriptionCreate = "Please fill the form below to create your Client";
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
        this.initClient();
        this.getTiers();
    }

    public initClient(): void {
        this.client = {
            clientId: "",
            displayName: "",
            clientSecret: ""
        } as Client;
    }

    public getTiers(): void {
        this.tierList = [];
        this.tierService.getTiers().subscribe({
            next: (data: PagedHttpResponseEnvelope<TierOverview>) => {
                this.tierList = data.result;
            },
            error: (err: any) => {
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public createClient(): void {
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
                this.clientId = data.result.clientId;
            },
            complete: () => this.clientService.triggerRefresh(),
            error: (err: any) => {
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public onDialogCancel(): void {
        this.dialogRef.close();
    }

    public togglePasswordVisibility(): void {
        this.showPassword = !this.showPassword;
    }

    public canSaveClient(): boolean {
        if (!this.client.defaultTier) return false;
        if (this.client.maxIdentities !== undefined && this.client.maxIdentities < 0) return false;
        return true;
    }
}
