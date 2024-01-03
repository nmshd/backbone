import { Component, Input } from "@angular/core";
import { PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MessageOverview, MessageService } from "src/app/services/message-service/message.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";

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
    public sentMessagesTableDisplayedColumns: string[];

    public messagesTableData: MessageOverview[];

    public messagesTotalRecords: number;
    public messagesPageSize: number;
    public messagesPageIndex: number;

    public loading: boolean;

    public constructor(
        private readonly snackBar: MatSnackBar,
        private readonly messageService: MessageService
    ) {
        this.showSentMessages = false;
        this.receivedMessagesTableDisplayedColumns = ["senderAddress", "senderDevice", "sendDate", "numberOfAttachments"];
        this.sentMessagesTableDisplayedColumns = ["recipients", "sendDate", "numberOfAttachments"];
        this.messagesTableData = [];
        this.messagesTotalRecords = 0;
        this.messagesPageSize = 10;
        this.messagesPageIndex = 0;
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
            this.messageService.getSentMessagesByParticipantAddress(this.identityAddress!, this.messagesPageIndex, this.messagesPageSize).subscribe({
                next: (data: PagedHttpResponseEnvelope<MessageOverview>) => {
                    this.messagesTableData = data.result;
                    this.messagesTotalRecords = data.pagination?.totalRecords ? data.pagination.totalRecords : data.result.length;
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
            this.messageService.getReceivedMessagesByParticipantAddress(this.identityAddress!, this.messagesPageIndex, this.messagesPageSize).subscribe({
                next: (data: PagedHttpResponseEnvelope<MessageOverview>) => {
                    this.messagesTableData = data.result;
                    this.messagesTotalRecords = data.pagination?.totalRecords ? data.pagination.totalRecords : data.result.length;
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

    public messagesPageChangeEvent(event: PageEvent): void {
        this.messagesPageIndex = event.pageIndex;
        this.messagesPageSize = event.pageSize;
        this.getMessages();
    }
}
