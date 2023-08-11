import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { XSRFService } from "../xsrf-service/xsrf.service";

@Injectable({
    providedIn: "root"
})
export class AuthService {
    private loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.hasApiKey());
    apiUrl: string;

    get isLoggedIn() {
        return this.loggedIn.asObservable();
    }

    constructor(private router: Router, private http: HttpClient, private xsrfService: XSRFService) {
        this.apiUrl = environment.apiUrl;
    }

    isCurrentlyLoggedIn(): boolean {
        return this.loggedIn.value;
    }

    hasApiKey(): boolean {
        return !!localStorage.getItem("api-key");
    }

    getApiKey(): string | null {
        return localStorage.getItem("api-key");
    }

    validateApiKey(apiKeyRequest: ValidateApiKeyRequest): Observable<ValidateApiKeyResponse> {
        return this.http.post<ValidateApiKeyResponse>(`${this.apiUrl}/ValidateApiKey`, apiKeyRequest, { headers: { skip: "true" } });
    }

    login(apiKey: string): void {
        localStorage.setItem("api-key", apiKey);
        this.xsrfService.loadAndStoreXSRFToken();
        this.loggedIn.next(true);
        this.router.navigate(["/"]);
    }

    logout(): void {
        localStorage.removeItem("api-key");
        this.loggedIn.next(false);
        this.xsrfService.clearStoredToken();
        this.router.navigate(["/login"]);
    }
}

export interface ValidateApiKeyResponse {
    isValid: boolean;
}

export interface ValidateApiKeyRequest {
    apiKey: string;
}
