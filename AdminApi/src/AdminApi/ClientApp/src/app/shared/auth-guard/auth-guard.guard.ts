import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
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

    public canActivate(): Observable<Promise<boolean>> {
        return this.authService.isLoggedIn.pipe(
            take(1),
            map(async (isLoggedIn: boolean) => {
                if (!isLoggedIn) {
                    await this.router.navigate(["/login"]);
                    return false;
                }
                return true;
            })
        );
    }
}
