import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LazyLoadEvent } from 'primeng/api';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class IdentityService {
    constructor(private http: HttpClient) {}

    getIdentities(event: LazyLoadEvent): Observable<IdentityDTO> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            }),
            params: new HttpParams()
                .set('skip', event.first!)
                .set('take', event.rows!)
                .set('sort', event.sortField!)
                .set('sortOrder', event.sortOrder!),
            filters: event.filters,
        };

        return this.http.get<IdentityDTO>('identity', httpOptions);
    }
}

export interface IdentityDTO {
    identities: Identity[];
    totalRecords: number;
}

export interface Identity {
    address?: string;
    clientId?: string;
    publicKey?: string;
    createdAt?: Date;
    identityVersion?: string;
}
