import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { from, lastValueFrom } from "rxjs";
import { XSRFService } from "src/app/services/xsrf-service/xsrf.service";

@Injectable()
export class XSRFInterceptor implements HttpInterceptor {
    constructor(private xsrfService: XSRFService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        return from(this.handle(req, next));
    }

    async handle(req: HttpRequest<any>, next: HttpHandler) {
        const token = this.xsrfService.getStoredToken();

        req = req.clone({
            withCredentials: true,
            setHeaders: {
                "X-XSRF-TOKEN": token
            }
        });

        return await lastValueFrom(next.handle(req));
    }
}
