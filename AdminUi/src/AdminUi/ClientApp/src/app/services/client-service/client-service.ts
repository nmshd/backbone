import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class ClientServiceService {
    apiUrl: string;
    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + '/Clients';
    }

    getClients(
        pageNumber: number,
        pageSize: number
    ): Observable<PagedHttpResponseEnvelope<ClientDTO>> {
        const httpOptions = {
            params: new HttpParams()
                .set('PageNumber', pageNumber + 1)
                .set('PageSize', pageSize),
        };

        return this.http.get<PagedHttpResponseEnvelope<ClientDTO>>(
            this.apiUrl,
            httpOptions
        );
    }

    createClient(client: Client): Observable<HttpResponseEnvelope<Client>> {
        return this.http.post<HttpResponseEnvelope<Client>>(this.apiUrl, client);
    }

    deleteClient(clientId: string): Observable<any> {
        const httpOptions = {
            params: new HttpParams()
                .set('clientId', clientId)
        };
        return this.http.post<HttpResponseEnvelope<any>>(this.apiUrl, httpOptions);
    }
}

export interface ClientDTO {
    clientId: string;
    displayName?: string;
}

export interface Client {
    clientId?: string;
    displayName: string;
    clientSecret?: string;
}
