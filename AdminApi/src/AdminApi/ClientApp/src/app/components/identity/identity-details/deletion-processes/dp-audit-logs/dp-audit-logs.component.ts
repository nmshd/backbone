import { Component } from "@angular/core";
import { Router } from "express";
import { IdentityService } from "src/app/services/identity-service/identity.service";

@Component({
    selector: "app-dp-audit-logs",
    templateUrl: "./dp-audit-logs.component.html",
    styleUrls: ["./dp-audit-logs.component.css"]
})
export class DeletionProcessAuditLogsComponent {
    public disabled;
    public identityAddress;
    public headerTitle;

    public constructor(
        private readonly router: Router,
        private readonly identityService: IdentityService
    ) {
        this.disabled = false;

        this.identityAddress = "";
        this.headerTitle = "Identity deletion process audit logs";
    }

    findIdentityAuditLogs() {
        // this.identityService.getDeletionProcessAuditLogsOfIdentity(this.identityAddress).subscribe({
        //     next: (data: HttpResponseEnvelope<DeletionProcess>) => {},
        //     complete: () => {},
        //     error: (err: any) => {
        //         const errorMessage = err.error?.error?.message ?? err.message;
        //     }
        // });
    }

    canFindIdentityAuditLogs(): boolean {
        return this.identityAddress.trim().length > 0;
    }
}
