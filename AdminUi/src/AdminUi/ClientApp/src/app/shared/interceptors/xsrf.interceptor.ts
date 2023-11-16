import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Observable, catchError, throwError } from "rxjs";
import { XSRFService } from "src/app/services/xsrf-service/xsrf.service";

@Injectable()
export class XSRFInterceptor implements HttpInterceptor {
    public constructor(
        private readonly xsrfService: XSRFService,
        private readonly snackBar: MatSnackBar
    ) {}

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const token = this.xsrfService.getStoredToken();

        req = req.clone({
            withCredentials: true,
            setHeaders: {
                // eslint-disable-next-line @typescript-eslint/naming-convention
                "X-XSRF-TOKEN": token
            }
        });

        return next.handle(req).pipe(
            catchError((err) => {
                const xsrfError = err && err.status === 400 && (err.error.detail as string).includes("xsrf-token-may-be-invalid");
                if (xsrfError) {
                    this.xsrfService.clearStoredToken();
                    this.xsrfService.loadAndStoreXSRFToken();

                    this.snackBar.open("There was a state error. Please try again.", "Dismiss", {
                        verticalPosition: "top",
                        horizontalPosition: "center"
                    });
                    err = err.error?.message || err.statusText;
                }

                return throwError(() => err);
            })
        );
    }
}
