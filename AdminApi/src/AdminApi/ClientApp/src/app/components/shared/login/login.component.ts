import { Component, OnInit } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { AuthService, ValidateApiKeyRequest, ValidateApiKeyResponse } from "src/app/services/auth-service/auth.service";

@Component({
    selector: "app-login",
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
    public apiKey: string;
    public loading: boolean;

    public constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly authService: AuthService
    ) {
        this.apiKey = "";
        this.loading = false;
    }

    public async ngOnInit(): Promise<void> {
        if (this.authService.isCurrentlyLoggedIn()) {
            await this.router.navigate(["/"]);
        }
    }

    public login(): void {
        this.loading = true;
        const apiKeyRequest: ValidateApiKeyRequest = {
            apiKey: this.apiKey
        };
        this.authService.validateApiKey(apiKeyRequest).subscribe({
            next: async (response: ValidateApiKeyResponse) => {
                if (response.isValid) {
                    await this.authService.login(this.apiKey);
                } else {
                    this.snackBar.open("Invalid API Key.", "Dismiss", {
                        verticalPosition: "top",
                        horizontalPosition: "center"
                    });
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.snackBar.open(err, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }
}
