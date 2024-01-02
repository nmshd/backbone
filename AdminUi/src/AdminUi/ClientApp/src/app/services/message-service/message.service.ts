import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class MessageService {
    private readonly apiUrl: string;

    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Messages`;
    }

    public getReceivedMessagesByParticipantAddress(address: string): Observable<HttpResponseEnvelope<ReceivedMessage[]>> {
        const httpOptions = {
            params: new HttpParams().set("Participant", address).set("Type", "Incoming")
        };

        return this.http.get<HttpResponseEnvelope<ReceivedMessage[]>>(this.apiUrl, httpOptions);
    }

    public getSentMessagesByParticipantAddress(address: string): Observable<HttpResponseEnvelope<SentMessage[]>> {
        const httpOptions = {
            params: new HttpParams().set("Participant", address).set("Type", "Outgoing")
        };

        return this.http.get<HttpResponseEnvelope<SentMessage[]>>(this.apiUrl, httpOptions);
    }
}

export interface ReceivedMessage {
    senderAddress: string;
    senderDevice: string;
    sendDate: Date;
    numberOfAttachments: number;
}

export interface SentMessage {
    recipents: string[];
    sendDate: Date;
    numberOfAttachments: number;
}
