import { Component, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { PageEvent } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { MessageOverview, MessageRecipients, MessageService } from "src/app/services/message-service/message.service";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { IdentityDetailsMessageRecipientsDialogComponent } from "../identity-details-message-recipients-dialog/identity-details-message-recipients-dialog.component";
import { DownloadMessageDialogComponent } from "../download-message-dialog/download-message-dialog.component";

@Component({
    selector: "app-identity-details-messages",
    templateUrl: "./identity-details-messages.component.html",
    styleUrl: "./identity-details-messages.component.css"
})
export class IdentityDetailsMessagesComponent {
    @Input() public identityAddress?: string;
    @Input() public type?: string;

    public messagesTableDisplayedColumns: string[];

    public messagesTableData: MessageOverview[];

    public messagesTotalRecords: number;
    public messagesPageSize: number;
    public messagesPageIndex: number;

    public loading: boolean;

    public constructor(
        private readonly router: Router,
        private readonly dialog: MatDialog,
        private readonly snackBar: MatSnackBar,
        private readonly messageService: MessageService
    ) {
        this.messagesTableDisplayedColumns = [];
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
                    this.messagesTableDisplayedColumns = ["recipents", "sendDate", "numberOfAttachments", "downloadMessage"];
                    break;
                case "Incoming":
                    this.messagesTableDisplayedColumns = ["senderAddress", "senderDevice", "sendDate", "numberOfAttachments", "downloadMessage"];
                    break;
            }
        }

        this.getMessages();
    }

    public getMessages(): void {
        this.loading = true;
        if (this.type) {
            this.messageService.getMessagesByParticipantAddress(this.identityAddress!, this.type, this.messagesPageIndex, this.messagesPageSize).subscribe({
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

    public openRecipientsDialog(recipients: MessageRecipients[]): void {
        const dialogRef = this.dialog.open(IdentityDetailsMessageRecipientsDialogComponent, {
            data: { recipients: recipients },
            width: "500px",
            maxWidth: "100%"
        });

        dialogRef.afterClosed().subscribe(async (result: string) => {
            if (result) {
                await this.goToIdentity(result);
            }
        });
    }

    public async goToIdentity(identityAddress: string): Promise<void> {
        await this.router.navigate([`/identities/${identityAddress}`]);
    }

    public openDownloadMessageDialog(): void {
        this.dialog.open(DownloadMessageDialogComponent, {
            maxWidth: "420px",
            maxHeight: "350px"
        });
    }
}
