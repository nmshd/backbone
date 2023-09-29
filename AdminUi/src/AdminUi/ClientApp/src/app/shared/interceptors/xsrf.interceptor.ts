import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, from, lastValueFrom } from "rxjs";
import { XSRFService } from "src/app/services/xsrf-service/xsrf.service";

@Injectable()
export class XSRFInterceptor implements HttpInterceptor {
    public constructor(private readonly xsrfService: XSRFService) {}

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return from(this.handle(req, next));
    }

    public async handle(req: HttpRequest<any>, next: HttpHandler): Promise<HttpEvent<any>> {
        const token = this.xsrfService.getStoredToken();

        req = req.clone({
            withCredentials: true,
            setHeaders: {
                // eslint-disable-next-line @typescript-eslint/naming-convention
                "X-XSRF-TOKEN": token
            }
        });

        return await lastValueFrom(next.handle(req));
    }
}
