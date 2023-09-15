import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from "@angular/router";
import { map, Observable, take } from "rxjs";
import { AuthService } from "src/app/services/auth-service/auth.service";

@Injectable({
    providedIn: "root"
})
export class AuthGuard {
    public constructor(
        private readonly authService: AuthService,
        private readonly router: Router
    ) {}

    public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.authService.isLoggedIn.pipe(
            take(1),
            map((isLoggedIn: boolean) => {
                if (!isLoggedIn) {
                    this.router.navigate(["/login"]);
                    return false;
                }
                return true;
            })
        );
    }
}
