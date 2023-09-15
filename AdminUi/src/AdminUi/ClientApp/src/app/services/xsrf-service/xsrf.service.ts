import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class XSRFService {
    apiUrl: string;

    constructor(private readonly http: HttpClient) {
        this.apiUrl = environment.apiUrl;
    }

    loadAndStoreXSRFToken(): void {
        this.http.get(`${this.apiUrl}/xsrf`, { responseType: "text" }).subscribe({
            next: (token) => {
                localStorage.setItem("xsrf-token", token);
            }
        });
    }

    getStoredToken(): string {
        return localStorage.getItem("xsrf-token") ?? "";
    }

    clearStoredToken(): void {
        localStorage.removeItem("xsrf-token");
    }
}
