import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Observable, catchError, throwError } from "rxjs";
import { AuthService } from "src/app/services/auth-service/auth.service";

@Injectable()
export class ApiKeyInterceptor implements HttpInterceptor {
    isLoggedIn$: Observable<boolean> | undefined;

    constructor(private authService: AuthService, private snackBar: MatSnackBar) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.isLoggedIn$ = this.authService.isLoggedIn;
        const skipIntercept = request.headers.has("skip");
        if (skipIntercept) {
            request = request.clone({
                headers: request.headers.delete("skip")
            });
        } else if (this.isLoggedIn$ && this.authService.getApiKey() != null) {
            request = request.clone({
                setHeaders: {
                    "X-API-KEY": this.authService.getApiKey()!
                }
            });
        }

        return next.handle(request).pipe(
            catchError((err) => {
                const isUnauthorized = err && err.status === 401;
                if (isUnauthorized) {
                    this.authService.logout().then((_) => {
                        this.snackBar.open("You are currently not authenticated. Please sign in.", "Dismiss", {
                            verticalPosition: "top",
                            horizontalPosition: "center"
                        });
                    });
                }

                const error = err.error?.message || err.statusText;
                return throwError(() => error);
            })
        );
    }
}
