import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class IdentityService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + '/Identities';
    }

    getIdentities(
        pageNumber: number,
        pageSize: number
    ): Observable<PagedHttpResponseEnvelope<Identity>> {
        const httpOptions = {
            params: new HttpParams()
                .set('PageNumber', pageNumber + 1)
                .set('PageSize', pageSize),
        };

        return this.http.get<PagedHttpResponseEnvelope<Identity>>(
            this.apiUrl,
            httpOptions
        );
    }
}

export interface Identity {
    address: string;
    clientId: string;
    publicKey: string;
    createdAt: Date;
    identityVersion: string;
}
