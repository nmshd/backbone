import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { DateFilter } from "src/app/utils/date-filter";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { NumberFilter } from "src/app/utils/number-filter";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";
import { TierDTO } from "../tier-service/tier.service";

@Injectable({
    providedIn: "root"
})
export class ClientService {
    private readonly apiUrl: string;
    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Clients`;
    }

    public getClientById(id: string): Observable<HttpResponseEnvelope<Client>> {
        return this.http.get<HttpResponseEnvelope<Client>>(`${this.apiUrl}/${id}`);
    }

    public getClients(): Observable<PagedHttpResponseEnvelope<ClientOverview>> {
        return this.http.get<PagedHttpResponseEnvelope<ClientOverview>>(this.apiUrl);
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
    displayName: string;
    defaultTier: TierDTO;
    createdAt: Date;
    maxIdentities?: number;
    numberOfIdentities: number;
}

export interface Client {
    clientId?: string;
    displayName: string;
    clientSecret?: string;
    defaultTier: string;
    createdAt: Date;
    maxIdentities?: number;
}

export interface ChangeClientSecretRequest {
    newSecret: string;
}

export interface UpdateClientRequest {
    defaultTier: string;
    maxIdentities: number;
}

export interface ClientOverviewFilter {
    clientId?: string;
    tiers?: string[];
    displayName?: string;
    numberOfIdentities: NumberFilter;
    createdAt: DateFilter;
}
