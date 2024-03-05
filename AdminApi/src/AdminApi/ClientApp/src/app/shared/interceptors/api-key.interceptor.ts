import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Observable, catchError, from, throwError } from "rxjs";
import { AuthService } from "src/app/services/auth-service/auth.service";

@Injectable()
export class ApiKeyInterceptor implements HttpInterceptor {
    private isLoggedIn$: Observable<boolean> | undefined;

    public constructor(
        private readonly authService: AuthService,
        private readonly snackBar: MatSnackBar
    ) {}

    public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.isLoggedIn$ = this.authService.isLoggedIn;
        const skipIntercept = request.headers.has("skip");
        if (skipIntercept) {
            request = request.clone({
                headers: request.headers.delete("skip")
            });
        } else {
            this.isLoggedIn$.subscribe((isLoggedIn) => {
                if (isLoggedIn && this.authService.getApiKey() !== null) {
                    request = request.clone({
                        setHeaders: {
                            // eslint-disable-next-line @typescript-eslint/naming-convention
                            "X-API-KEY": this.authService.getApiKey()!
                        }
                    });
                }
            });
        }

        return next.handle(request).pipe(
            catchError((err) => {
                const isUnauthorized = err && err.status === 401;
                if (isUnauthorized) {
                    from(
                        this.authService.logout().then((_) => {
                            this.snackBar.open("You are currently not authenticated. Please sign in.", "Dismiss", {
                                verticalPosition: "top",
                                horizontalPosition: "center"
                            });
                        })
                    );
                    err = err.error?.message || err.statusText;
                }

                return throwError(() => err);
            })
        );
    }
}
