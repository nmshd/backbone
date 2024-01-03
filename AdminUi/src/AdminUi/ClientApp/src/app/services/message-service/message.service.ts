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

    public getReceivedMessagesByParticipantAddress(address: string, pageNumber: number, pageSize: number): Observable<PagedHttpResponseEnvelope<MessageOverview>> {
        const httpOptions = {
            params: new HttpParams()
                .set("Participant", address)
                .set("Type", "Incoming")
                .set("PageNumber", pageNumber + 1)
                .set("PageSize", pageSize)
        };

        return this.http.get<PagedHttpResponseEnvelope<MessageOverview>>(this.apiUrl, httpOptions);
    }

    public getSentMessagesByParticipantAddress(address: string, pageNumber: number, pageSize: number): Observable<PagedHttpResponseEnvelope<MessageOverview>> {
        const httpOptions = {
            params: new HttpParams()
                .set("Participant", address)
                .set("Type", "Outgoing")
                .set("PageNumber", pageNumber + 1)
                .set("PageSize", pageSize)
        };

        return this.http.get<PagedHttpResponseEnvelope<MessageOverview>>(this.apiUrl, httpOptions);
    }
}

export interface MessageOverview {
    senderAddress: string;
    senderDevice: string;
    sendDate: Date;
    numberOfAttachments: number;
    recipients: string[];
}
