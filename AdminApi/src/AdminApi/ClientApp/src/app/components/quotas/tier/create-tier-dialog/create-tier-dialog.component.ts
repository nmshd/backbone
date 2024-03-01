import { Component } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Tier, TierService } from "src/app/services/tier-service/tier.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-create-tier-dialog",
    templateUrl: "./create-tier-dialog.component.html",
    styleUrl: "./create-tier-dialog.component.css"
})
export class CreateTierDialogComponent {
    public headerCreate: string;
    public headerDescriptionCreate: string;
    public disabled: boolean;
    public tierId?: string;
    public tier: Tier;

    public constructor(
        public readonly dialogRef: MatDialogRef<CreateTierDialogComponent>,
        private readonly tierService: TierService,
        private readonly snackBar: MatSnackBar
    ) {
        this.headerCreate = "Create Tier";
        this.headerDescriptionCreate = "Please fill the form below to create your Tier";
        this.disabled = false;
        this.tier = {
            id: "",
            name: "",
            quotas: [],
            isDeletable: false,
            numberOfIdentities: 0,
            isReadOnly: false
        } as Tier;
    }

    public ngOnInit(): void {
        this.initTier();
    }

    public initTier(): void {
        this.tier = {
            name: ""
        } as Tier;
    }

    public validateTier(): boolean {
        if (this.tier.name.length > 0) {
            return true;
        }
        return false;
    }

    public createTier(): void {
        this.tierService.createTier(this.tier).subscribe({
            next: (data: HttpResponseEnvelope<Tier>) => {
                this.tier = {
                    id: data.result.id,
                    name: data.result.name,
                    quotas: [],
                    numberOfIdentities: data.result.numberOfIdentities,
                    isDeletable: data.result.isDeletable,
                    isReadOnly: false
                } as Tier;

                this.snackBar.open("Successfully added tier.", "Dismiss", {
                    duration: 4000,
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
                this.dialogRef.close(true);
            },
            error: (err: any) => {
                const errorMessage = err.error?.error?.message ?? err.message;
                this.snackBar.open(errorMessage, "Dismiss", {
                    verticalPosition: "top",
                    horizontalPosition: "center"
                });
            }
        });
    }

    public closeTierEditDialog(): void {
        this.dialogRef.close();
    }
}
