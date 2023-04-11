import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LazyLoadEvent } from 'primeng/api';
import { Observable } from 'rxjs';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class TierService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + '/Tiers';
    }

    getTiers(
        event: LazyLoadEvent
    ): Observable<PagedHttpResponseEnvelope<Tier>> {
        const httpOptions = {
            params: new HttpParams()
                .set('PageNumber', event.first! / event.rows! + 1)
                .set('PageSize', event.rows!),
        };

        return this.http.get<PagedHttpResponseEnvelope<Tier>>(
            this.apiUrl,
            httpOptions
        );
    }

    getTierById(id: string): Observable<HttpResponseEnvelope<Tier>> {
        const httpOptions = {
            params: new HttpParams().set('id', id),
        };

        return this.http.get<HttpResponseEnvelope<Tier>>(
            this.apiUrl,
            httpOptions
        );
    }

    createTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.post<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }

    updateTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.put<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }
}

export interface Tier {
    id?: string;
    name?: string;
}
