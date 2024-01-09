import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MessageRecipients } from "src/app/services/message-service/message.service";

@Component({
    selector: "app-identity-details-message-recipients-dialog",
    templateUrl: "./identity-details-message-recipients-dialog.component.html",
    styleUrl: "./identity-details-message-recipients-dialog.component.css"
})
export class IdentityDetailsMessageRecipientsDialogComponent {
    public header: string;
    public messagesRecipients: MessageRecipients[];

    public constructor(
        @Inject(MAT_DIALOG_DATA) public recipients: any,
        public dialogRef: MatDialogRef<IdentityDetailsMessageRecipientsDialogComponent>
    ) {
        this.header = "Recipients";
        this.messagesRecipients = recipients.recipients;
    }

    public returnIdentity(identityAddress: string): void {
        this.dialogRef.close(identityAddress);
    }
}
