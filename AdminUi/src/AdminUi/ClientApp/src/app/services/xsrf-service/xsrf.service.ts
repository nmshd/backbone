import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CookieService } from "ngx-cookie-service";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class XSRFService {
    private readonly apiUrl: string;

    public constructor(
        private readonly http: HttpClient,
        private readonly cookieService: CookieService
    ) {
        this.apiUrl = environment.apiUrl;
    }

    public loadAndStoreXSRFToken(): void {
        this.http.get(`${this.apiUrl}/xsrf`, { responseType: "text" }).subscribe({
            next: (token) => {
                localStorage.setItem("xsrf-token", token);
            }
        });
    }

    public getStoredToken(): string {
        return localStorage.getItem("xsrf-token") ?? "";
    }

    public clearStoredToken(): void {
        localStorage.removeItem("xsrf-token");
        this.cookieService.delete("X-XSRF-COOKIE");
    }
}
