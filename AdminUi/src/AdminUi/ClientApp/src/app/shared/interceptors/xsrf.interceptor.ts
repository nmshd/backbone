import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import "cookie-store";
import { from, lastValueFrom } from "rxjs";
import { AuthService } from "src/app/services/auth-service/auth.service";

@Injectable()
export class XSRFInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        return from(this.handle(req, next));
    }

    async handle(req: HttpRequest<any>, next: HttpHandler) {
        const token = localStorage.getItem("xsrf-token");
        if (token) {
            req = req.clone({
                withCredentials: true,
                setHeaders: {
                    "X-XSRF-TOKEN": token
                }
            });
        }
        return await lastValueFrom(next.handle(req));
    }
}
