import { Component } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute } from "@angular/router";
import { Identity, IdentityService } from "src/app/services/identity-service/identity.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { AssignQuotaData, AssignQuotasDialogComponent } from "../../assign-quotas-dialog/assign-quotas-dialog.component";
import { CreateQuotaForIdentityRequest, IdentityQuota, QuotasService } from "src/app/services/quotas-service/quotas.service";
import { MatDialog } from "@angular/material/dialog";

@Component({
    selector: "app-identity-edit",
    templateUrl: "./identity-edit.component.html",
    styleUrls: ["./identity-edit.component.css"]
})
export class IdentityEditComponent {
    header: string;
    headerDescription: string;
    headerQuotas: string;
    headerQuotasDescription: string;
    identityAddress?: string;
    disabled: boolean;
    identity: Identity;
    loading: boolean;

    constructor(private route: ActivatedRoute, private snackBar: MatSnackBar, private dialog: MatDialog, private identityService: IdentityService, private quotasService: QuotasService) {
        this.header = "Edit Identity";
        this.headerDescription = "Perform your desired changes for this Identity";
        this.headerQuotas = "Quotas";
        this.headerQuotasDescription = "View and assign quotas for this Identity.";
        this.loading = true;
        this.disabled = false;
        this.identity = {} as Identity;
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            if (params["address"]) {
                this.identityAddress = params["address"];
            }
        });

        this.getIdentity();
    }

    getIdentity() {
        this.loading = true;

        this.identityService.getIdentityByAddress(this.identityAddress!).subscribe({
            next: (data: HttpResponseEnvelope<Identity>) => {
                if (data && data.result) {
                    this.identity = data.result;
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.snackBar.open(err.message, "Dismiss", {
                    panelClass: ["snack-bar"]
                });
            }
        });
    }

    openAssignQuotaDialog() {
        let dialogRef = this.dialog.open(AssignQuotasDialogComponent, {
            minWidth: "50%"
        });

        dialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.createIdentityQuota(result);
            }
        });
    }

    createIdentityQuota(quotaData: AssignQuotaData) {
        this.loading = true;

        const createQuotaRequest = {
            metricKey: quotaData.metricKey,
            max: quotaData.max,
            period: quotaData.period
        } as CreateQuotaForIdentityRequest;

        this.quotasService.createIdentityQuota(createQuotaRequest, this.identity.address).subscribe({
            next: (data: HttpResponseEnvelope<IdentityQuota>) => {
                if (data && data.result) {
                    this.snackBar.open("Successfully assigned quota.", "Dismiss", {
                        panelClass: ["snack-bar"]
                    });
                }
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                this.snackBar.open(err.message, "Dismiss", {
                    panelClass: ["snack-bar"]
                });
            }
        });
    }
}
