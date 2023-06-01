import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class QuotasService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + '/Quotas';
    }

    getQuotas(
        pageNumber: number,
        pageSize: number
    ): Observable<PagedHttpResponseEnvelope<Quota>> {
        const httpOptions = {
            params: new HttpParams()
                .set('PageNumber', pageNumber + 1)
                .set('PageSize', pageSize),
        };

        return this.http.get<PagedHttpResponseEnvelope<Quota>>(
            this.apiUrl,
            httpOptions
        );
    }

    getMetrics(): Observable<Metric[]> {
        //return this.http.get<Metric[]>(this.apiUrl);
        //Dummy data
        let metric: Metric = {
            id: '1',
            key: 'A',
            displayName: 'Sent Messages',
        };
        return of([metric]);
    }
}

export interface Quota {
    id?: string;
    metric?: Metric;
    max?: number;
    validFrom?: Date;
    validTo?: Date;
    period?: string;
}

export interface Metric {
    id?: string;
    key?: string;
    displayName?: string;
}
