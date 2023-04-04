import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LazyLoadEvent } from 'primeng/api';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class TierService {
    constructor(private http: HttpClient) {}

    getTiers(event: LazyLoadEvent): Observable<TierDTO> {
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

        return this.http.get<TierDTO>('tier', httpOptions);
    }

    getTierById(id: string): Observable<Tier> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            }),
            params: new HttpParams().set('id', id),
        };

        return this.http.get<Tier>('tier', httpOptions);
    }

    createTier(tier: Tier): Observable<Tier> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            }),
        };

        return this.http.post<Tier>('tier', tier, httpOptions);
    }

    updateTier(tier: Tier): Observable<Tier> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            }),
        };

        return this.http.put<Tier>('tier', tier, httpOptions);
    }
}

export interface TierDTO {
    tiers: Tier[];
    totalRecords: number;
}

export interface Tier {
    id?: string;
    name?: string;
    // quotas?: Quotas[];
}
