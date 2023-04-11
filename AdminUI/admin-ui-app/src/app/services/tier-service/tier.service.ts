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
            params: new HttpParams()
                .set('PageNumber', event.first! / event.rows! + 1)
                .set('PageSize', event.rows!),
        };

        return this.http.get<TierDTO>(
            'http://localhost:5010/api/v1/Tiers',
            httpOptions
        );
    }

    getTierById(id: string): Observable<Tier> {
        const httpOptions = {
            params: new HttpParams().set('id', id),
        };

        return this.http.get<Tier>(
            'http://localhost:5010/api/v1/Tiers',
            httpOptions
        );
    }

    createTier(tier: Tier): Observable<Tier> {
        return this.http.post<Tier>('http://localhost:5010/api/v1/Tiers', tier);
    }

    updateTier(tier: Tier): Observable<Tier> {
        return this.http.put<Tier>('http://localhost:5010/api/v1/Tiers', tier);
    }
}

export interface TierDTO {
    result: Tier[];
    pagination: Pagination;
}

export interface Tier {
    id?: string;
    name?: string;
}

interface Pagination {
    pageNumber?: number;
    pageSize?: number;
    totalPages?: number;
    totalRecords?: number;
}
