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
    private readonly loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.hasApiKey());
    private readonly apiUrl: string;

    public get isLoggedIn(): Observable<boolean> {
        return this.loggedIn.asObservable();
    }

    public constructor(
        private readonly router: Router,
        private readonly http: HttpClient,
        private readonly xsrfService: XSRFService
    ) {
        this.apiUrl = environment.apiUrl;
    }

    public isCurrentlyLoggedIn(): boolean {
        return this.loggedIn.value;
    }

    public hasApiKey(): boolean {
        return !!localStorage.getItem("api-key");
    }

    public getApiKey(): string | null {
        return localStorage.getItem("api-key");
    }

    public validateApiKey(apiKeyRequest: ValidateApiKeyRequest): Observable<ValidateApiKeyResponse> {
        return this.http.post<ValidateApiKeyResponse>(`${this.apiUrl}/ValidateApiKey`, apiKeyRequest, { headers: { skip: "true" } });
    }

    public async login(apiKey: string): Promise<void> {
        localStorage.setItem("api-key", apiKey);
        this.xsrfService.loadAndStoreXSRFToken();
        this.loggedIn.next(true);
        await this.router.navigate(["/"]);
    }

    public async logout(): Promise<boolean> {
        localStorage.removeItem("api-key");
        localStorage.removeItem("breadcrumb-history");
        this.loggedIn.next(false);
        this.xsrfService.clearStoredToken();
        return await this.router.navigate(["/login"]);
    }
}

export interface ValidateApiKeyResponse {
    isValid: boolean;
}

export interface ValidateApiKeyRequest {
    apiKey: string;
}
