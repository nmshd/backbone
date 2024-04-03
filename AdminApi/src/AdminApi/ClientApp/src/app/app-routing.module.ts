import { NgModule } from "@angular/core";
import { RouteReuseStrategy, RouterModule, Routes } from "@angular/router";
import { ClientEditComponent } from "./components/client/client-edit/client-edit.component";
import { ClientListComponent } from "./components/client/client-list/client-list.component";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { PageNotFoundComponent } from "./components/error/page-not-found/page-not-found.component";
import { DeletionProcessDetailsComponent } from "./components/identity/identity-details/deletion-processes/dp-details/dp-details.component";
import { IdentityDetailsComponent } from "./components/identity/identity-details/identity-details.component";
import { IdentityListComponent } from "./components/identity/identity-list/identity-list.component";
import { TierEditComponent } from "./components/quotas/tier/tier-edit/tier-edit.component";
import { TierListComponent } from "./components/quotas/tier/tier-list/tier-list.component";
import { LoginComponent } from "./components/shared/login/login.component";
import { AuthGuard } from "./shared/auth-guard/auth-guard.guard";
import { CustomRouteReuseStrategy } from "./utils/custom-route-reuse-strategy";

const routes: Routes = [
    { path: "", redirectTo: "/dashboard", pathMatch: "full" },
    { path: "login", component: LoginComponent },
    { path: "dashboard", component: DashboardComponent, data: { breadcrumb: "Dashboard" }, canActivate: [AuthGuard] },
    { path: "identities", component: IdentityListComponent, data: { breadcrumb: "Identities" }, canActivate: [AuthGuard] },
    { path: "identities/:address", component: IdentityDetailsComponent, canActivate: [AuthGuard] },
    { path: "tiers", component: TierListComponent, data: { breadcrumb: "Tiers" }, canActivate: [AuthGuard] },
    { path: "tiers/create", component: TierEditComponent, data: { breadcrumb: "Create Tier" }, canActivate: [AuthGuard] },
    { path: "tiers/:id", component: TierEditComponent, canActivate: [AuthGuard] },
    { path: "clients", component: ClientListComponent, data: { breadcrumb: "Clients" }, canActivate: [AuthGuard] },
    { path: "clients/create", component: ClientEditComponent, data: { breadcrumb: "Create Client" }, canActivate: [AuthGuard] },
    { path: "clients/:id", component: ClientEditComponent, canActivate: [AuthGuard] },
    { path: "deletion-process-details/:address/:id", component: DeletionProcessDetailsComponent, data: { breadcrumb: "Identity Deletion Process" }, canActivate: [AuthGuard] },
    { path: "**", component: PageNotFoundComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: "reload" })],
    exports: [RouterModule],
    providers: [{ provide: RouteReuseStrategy, useClass: CustomRouteReuseStrategy }]
})
export class AppRoutingModule {}
