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

    public getClientById(id: string): Observable<HttpResponseEnvelope<ClientDTO>> {
        return this.http.get<HttpResponseEnvelope<ClientDTO>>(`${this.apiUrl}/${id}`);
    }

    public getClients(pageNumber: number, pageSize: number): Observable<PagedHttpResponseEnvelope<ClientDTO>> {
        const httpOptions = {
            params: new HttpParams().set("PageNumber", pageNumber + 1).set("PageSize", pageSize)
        };

        return this.http.get<PagedHttpResponseEnvelope<ClientDTO>>(this.apiUrl, httpOptions);
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

    public updateClient(clientId: string, request: UpdateClientRequest): Observable<HttpResponseEnvelope<ClientDTO>> {
        return this.http.patch<HttpResponseEnvelope<ClientDTO>>(`${this.apiUrl}/${clientId}`, request);
    }
}

export interface ClientDTO {
    clientId: string;
    displayName?: string;
    defaultTier: string;
}

export interface Client {
    clientId?: string;
    displayName: string;
    clientSecret?: string;
    defaultTier: string;
}

export interface ChangeClientSecretRequest {
    newSecret: string;
}

export interface UpdateClientRequest {
    defaultTier: string;
}
