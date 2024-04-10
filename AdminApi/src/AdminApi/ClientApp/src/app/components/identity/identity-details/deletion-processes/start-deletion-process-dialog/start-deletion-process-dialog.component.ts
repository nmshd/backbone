import { Component } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
    selector: "app-start-deletion-process-dialog",
    templateUrl: "./start-deletion-process-dialog.component.html",
    styleUrl: "./start-deletion-process-dialog.component.css"
})
export class StartDeletionProcessDialogComponent {
    public constructor(public dialogRef: MatDialogRef<StartDeletionProcessDialogComponent>) {}

    public startDeletionProcess(): void {
        this.dialogRef.close(true);
    }
}
