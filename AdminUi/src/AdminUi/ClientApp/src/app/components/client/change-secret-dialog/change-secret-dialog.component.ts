import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ChangeClientSecretRequest, Client, ClientServiceService } from "src/app/services/client-service/client-service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-change-secret-dialog",
    templateUrl: "./change-secret-dialog.component.html",
    styleUrls: ["./change-secret-dialog.component.css"]
})
export class ChangeSecretDialogComponent {
    header: string;
    clientId: string;
    clientSecret: string;
    showSecret: boolean;
    displayClientSecretWarning: boolean;
    loading: boolean;
    disabled: boolean;

    constructor(
        private snackBar: MatSnackBar,
        private clientService: ClientServiceService,
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

    ngOnInit() {
        this.loading = false;
        this.clientId = this.data.clientId;

        if (!this.data.clientId) {
            this.disabled = true;
        }
    }

    changeSecret() {
        this.loading = true;
        this.disabled = true;
        let request = {
            newSecret: this.clientSecret
        } as ChangeClientSecretRequest;

        this.clientService.changeClientSecret(this.clientId, request).subscribe({
            next: (data: HttpResponseEnvelope<Client>) => {
                if (data && data.result) {
                    this.clientSecret = data.result.clientSecret!;
                }
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
                let errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    togglePasswordVisibility(): void {
        this.showSecret = !this.showSecret;
    }
}
