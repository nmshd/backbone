import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { PageNotFoundComponent } from "./components/error/page-not-found/page-not-found.component";
import { IdentityListComponent } from "./components/quotas/identity/identity-list/identity-list.component";
import { TierListComponent } from "./components/quotas/tier/tier-list/tier-list.component";
import { TierEditComponent } from "./components/quotas/tier/tier-edit/tier-edit.component";
import { ClientListComponent } from "./components/client/client-list/client-list.component";
import { IdentityDetailsComponent } from "./components/quotas/identity/identity-details/identity-details.component";
import { ClientEditComponent } from "./components/client/client-edit/client-edit.component";
import { AuthGuard } from "./shared/auth-guard/auth-guard.guard";
import { LoginComponent } from "./components/shared/login/login.component";

const routes: Routes = [
    { path: "", redirectTo: "/dashboard", pathMatch: "full" },
    { path: "login", component: LoginComponent },
    { path: "dashboard", component: DashboardComponent, canActivate: [AuthGuard] },
    { path: "identities", component: IdentityListComponent, canActivate: [AuthGuard] },
    { path: "identities/:address", component: IdentityDetailsComponent, canActivate: [AuthGuard] },
    { path: "tiers", component: TierListComponent, canActivate: [AuthGuard] },
    { path: "tiers/create", component: TierEditComponent, canActivate: [AuthGuard] },
    { path: "tiers/:id", component: TierEditComponent, canActivate: [AuthGuard] },
    { path: "clients", component: ClientListComponent, canActivate: [AuthGuard] },
    { path: "clients/create", component: ClientEditComponent, canActivate: [AuthGuard] },
    { path: "clients/:id", component: ClientEditComponent, canActivate: [AuthGuard] },
    { path: "**", component: PageNotFoundComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {}
