import { Component } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
    selector: "app-cancel-deletion-process-dialog",
    templateUrl: "./cancel-deletion-process-dialog.component.html",
    styleUrl: "./cancel-deletion-process-dialog.component.css"
})
export class CancelDeletionProcessDialogComponent {

    public constructor(
        public dialogRef: MatDialogRef<CancelDeletionProcessDialogComponent>,
    ) {
    }

    public cancelDeletionProcess(): void {
        this.dialogRef.close(true);
    }
}
