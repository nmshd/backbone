import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class XSRFService {
    apiUrl: string;

    constructor(private http: HttpClient) {
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
        const token = localStorage.getItem("xsrf-token");
        if (!token) {
            throw new Error("The xsrf token was not found");
        }
        return token;
    }
}
