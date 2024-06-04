import { Component } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { IdentityService } from "src/app/services/identity-service/identity.service";
@Component({
    selector: "app-dp-audit-logs",
    templateUrl: "./dp-audit-logs.component.html",
    styleUrls: ["./dp-audit-logs.component.css"]
})
export class DeletionProcessAuditLogsComponent {
    public disabled: boolean;
    public identityAddress: string;
    public headerTitle: string;

    public constructor(
        private readonly router: Router,
        private readonly snackBar: MatSnackBar,
        private readonly identityService: IdentityService
    ) {
        this.disabled = false;

        this.identityAddress = "";
        this.headerTitle = "Identity deletion process audit logs";
    }

    public async navigateToIdentityDeletionProcessAuditLogs(): Promise<void> {
        await this.router.navigate([`identities/${this.identityAddress}/deletion-processes/audit-logs`]);
    }

    public isNotEmptyIdentityAddress(): boolean {
        return this.identityAddress.trim().length > 0;
    }
}
