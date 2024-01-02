import { Component, Input } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MessageService, ReceivedMessage, SentMessage } from "src/app/services/message-service/message.service";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";

@Component({
    selector: "app-identity-details-messages",
    templateUrl: "./identity-details-messages.component.html",
    styleUrl: "./identity-details-messages.component.css"
})
export class IdentityDetailsMessagesComponent {
    @Input() public identityAddress?: string;
    @Input() public type?: string;

    public showSentMessages: boolean;

    public receivedMessagesTableDisplayedColumns: string[];
    public receivedMessagesTableData: ReceivedMessage[];

    public sentMessagesTableDisplayedColumns: string[];
    public sentMessagesTableData: SentMessage[];

    public loading: boolean;

    public constructor(
        private readonly snackBar: MatSnackBar,
        private readonly messageService: MessageService
    ) {
        this.showSentMessages = false;
        this.receivedMessagesTableDisplayedColumns = ["senderAddress", "senderDevice", "sendDate", "numberOfAttachments"];
        this.receivedMessagesTableData = [];
        this.sentMessagesTableDisplayedColumns = ["recipients", "sendDate", "numberOfAttachments"];
        this.sentMessagesTableData = [];
        this.loading = false;
    }

    public ngOnInit(): void {
        if (this.type) {
            switch (this.type) {
                case "Outgoing":
                    this.showSentMessages = true;
                    break;
                case "Incoming":
                    this.showSentMessages = false;
                    break;
            }
        }

        this.getMessages();
    }

    public getMessages(): void {
        this.loading = true;

        if (this.showSentMessages) {
            this.messageService.getSentMessagesByParticipantAddress(this.identityAddress!).subscribe({
                next: (data: HttpResponseEnvelope<SentMessage[]>) => {
                    this.sentMessagesTableData = data.result;
                },
                complete: () => (this.loading = false),
                error: (err: any) => {
                    this.loading = false;
                    const errorMessage = err.error?.error?.message ?? err.message;
                    this.snackBar.open(errorMessage, "Dismiss", {
                        verticalPosition: "top",
                        horizontalPosition: "center"
                    });
                }
            });
        } else {
            this.messageService.getReceivedMessagesByParticipantAddress(this.identityAddress!).subscribe({
                next: (data: HttpResponseEnvelope<ReceivedMessage[]>) => {
                    this.receivedMessagesTableData = data.result;
                },
                complete: () => (this.loading = false),
                error: (err: any) => {
                    this.loading = false;
                    const errorMessage = err.error?.error?.message ?? err.message;
                    this.snackBar.open(errorMessage, "Dismiss", {
                        verticalPosition: "top",
                        horizontalPosition: "center"
                    });
                }
            });
        }
    }
}
