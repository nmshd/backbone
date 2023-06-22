import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { PageNotFoundComponent } from './components/error/page-not-found/page-not-found.component';
import { IdentityListComponent } from './components/quotas/identity/identity-list/identity-list.component';
import { TierListComponent } from './components/quotas/tier/tier-list/tier-list.component';
import { TierEditComponent } from './components/quotas/tier/tier-edit/tier-edit.component';
import { ClientListComponent } from './components/client/client-list/client-list.component';
import { IdentityEditComponent } from './components/quotas/identity/identity-edit/identity-edit.component';

const routes: Routes = [
    { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: DashboardComponent },
    { path: 'identities', component: IdentityListComponent },
    { path: 'identities/:address', component: IdentityEditComponent },
    { path: 'tiers', component: TierListComponent },
    { path: 'tiers/create', component: TierEditComponent },
    { path: 'tiers/:id', component: TierEditComponent },
    { path: 'clients', component: ClientListComponent },
    { path: '**', component: PageNotFoundComponent },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
