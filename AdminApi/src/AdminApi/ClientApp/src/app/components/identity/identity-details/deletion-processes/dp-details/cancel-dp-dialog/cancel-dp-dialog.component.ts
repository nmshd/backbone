import { Component } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
    selector: "app-cancel-deletion-process-dialog",
    templateUrl: "./cancel-dp-dialog.component.html",
    styleUrl: "./cancel-dp-dialog.component.css"
})
export class CancelDeletionProcessDialogComponent {
    public constructor(public dialogRef: MatDialogRef<CancelDeletionProcessDialogComponent>) {}

    public cancelDeletionProcess(): void {
        this.dialogRef.close(true);
    }
}
