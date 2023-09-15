import { Component, OnInit } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { AuthService, ValidateApiKeyRequest, ValidateApiKeyResponse } from "src/app/services/auth-service/auth.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-login",
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
    apiKey: string;
    loading: boolean;

    constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly authService: AuthService
    ) {
        this.apiKey = "";
        this.loading = false;
    }

    ngOnInit(): void {
        if (this.authService.isCurrentlyLoggedIn()) {
            this.router.navigate(["/"]);
        }
    }

    login(): void {
        this.loading = true;
        const apiKeyRequest: ValidateApiKeyRequest = {
            apiKey: this.apiKey
        };
        this.authService.validateApiKey(apiKeyRequest).subscribe({
            next: (response: ValidateApiKeyResponse) => {
                if (response.isValid) {
                    this.authService.login(this.apiKey);
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
