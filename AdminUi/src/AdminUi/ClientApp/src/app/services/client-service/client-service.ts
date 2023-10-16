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

    public getClientById(id: string): Observable<HttpResponseEnvelope<Client>> {
        return this.http.get<HttpResponseEnvelope<Client>>(`${this.apiUrl}/${id}`);
    }

    public getClients(pageNumber: number, pageSize: number): Observable<PagedHttpResponseEnvelope<ClientOverview>> {
        const httpOptions = {
            params: new HttpParams().set("PageNumber", pageNumber + 1).set("PageSize", pageSize)
        };

        return this.http.get<PagedHttpResponseEnvelope<ClientOverview>>(this.apiUrl, httpOptions);
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

    public updateClient(clientId: string, request: UpdateClientRequest): Observable<HttpResponseEnvelope<Client>> {
        return this.http.patch<HttpResponseEnvelope<Client>>(`${this.apiUrl}/${clientId}`, request);
    }
}

export interface ClientOverview {
    clientId: string;
    displayName?: string;
    defaultTier: string;
    createdAt: Date;
    numberOfIdentities: number;
}

export interface Client {
    clientId?: string;
    displayName: string;
    clientSecret?: string;
    defaultTier: string;
    createdAt: Date;
}

export interface ChangeClientSecretRequest {
    newSecret: string;
}

export interface UpdateClientRequest {
    defaultTier: string;
}
