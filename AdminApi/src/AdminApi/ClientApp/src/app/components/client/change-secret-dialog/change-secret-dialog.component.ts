import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ChangeClientSecretRequest, Client, ClientService } from "src/app/services/client-service/client-service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-change-secret-dialog",
    templateUrl: "./change-secret-dialog.component.html",
    styleUrls: ["./change-secret-dialog.component.css"]
})
export class ChangeSecretDialogComponent {
    public header: string;
    public clientId: string;
    public clientSecret: string;
    public showSecret: boolean;
    public displayClientSecretWarning: boolean;
    public loading: boolean;
    public disabled: boolean;

    public constructor(
        private readonly snackBar: MatSnackBar,
        private readonly clientService: ClientService,
        public dialogRef: MatDialogRef<ChangeSecretDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.header = "Change secret";
        this.clientId = "";
        this.clientSecret = "";
        this.showSecret = false;
        this.displayClientSecretWarning = false;
        this.loading = true;
        this.disabled = false;
    }

    public ngOnInit(): void {
        this.loading = false;
        this.clientId = this.data.clientId;

        if (!this.data.clientId) {
            this.disabled = true;
        }
    }

    public changeSecret(): void {
        this.loading = true;
        this.disabled = true;
        const request = {
            newSecret: this.clientSecret
        } as ChangeClientSecretRequest;

        this.clientService.changeClientSecret(this.clientId, request).subscribe({
            next: (data: HttpResponseEnvelope<Client>) => {
                this.clientSecret = data.result.clientSecret!;
                this.displayClientSecretWarning = true;
                this.disabled = true;
                this.snackBar.open("Successfully changed client secret.", "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.disabled = false;
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public togglePasswordVisibility(): void {
        this.showSecret = !this.showSecret;
    }
}
