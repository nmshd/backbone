import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class MessageService {
    private readonly apiUrl: string;

    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Messages`;
    }

    public getMessagesByParticipantAddress(address: string, type: string, pageNumber: number, pageSize: number): Observable<PagedHttpResponseEnvelope<MessageOverview>> {
        const httpOptions = {
            params: new HttpParams()
                .set("Participant", address)
                .set("Type", type)
                .set("PageNumber", pageNumber + 1)
                .set("PageSize", pageSize)
        };

        return this.http.get<PagedHttpResponseEnvelope<MessageOverview>>(this.apiUrl, httpOptions);
    }
}

export interface MessageOverview {
    messageId: string;
    senderAddress: string;
    senderDevice: string;
    sendDate: Date;
    numberOfAttachments: number;
    recipients: MessageRecipients[];
}

export interface MessageRecipients {
    messageId: string;
    address: string;
}
