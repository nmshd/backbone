import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class ClientServiceService {
    private readonly apiUrl: string;
    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Clients`;
    }

    public getClients(): Observable<PagedHttpResponseEnvelope<ClientDTO>> {
        return this.http.get<PagedHttpResponseEnvelope<ClientDTO>>(this.apiUrl);
    }

    public createClient(client: Client): Observable<HttpResponseEnvelope<Client>> {
        return this.http.post<HttpResponseEnvelope<Client>>(this.apiUrl, client);
    }

    public deleteClient(clientId: string): Observable<any> {
        return this.http.delete<HttpResponseEnvelope<any>>(`${this.apiUrl}/${clientId}`);
    }

    public changeClientSecret(clientId: string, request: ChangeClientSecretRequest): Observable<HttpResponseEnvelope<Client>> {
        return this.http.patch<HttpResponseEnvelope<Client>>(`${this.apiUrl}/${clientId}/ChangeSecret`, request);
    }
}

export interface ClientDTO {
    clientId: string;
    displayName: string;
}

export interface Client {
    clientId?: string;
    displayName: string;
    clientSecret?: string;
}

export interface ChangeClientSecretRequest {
    newSecret?: string;
}
