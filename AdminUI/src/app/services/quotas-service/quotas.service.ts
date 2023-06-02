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

    getPeriods(): string[] {
        return ['Monthly', 'Yearly'];
    }

    createTierQuota(quota: Quota, tierId: string): Observable<Quota> {
        //return this.http.post<Quota>(this.apiUrl, quota);
        //Dummy data
        return of(quota);
    }
}

export interface Quota {
    id?: string;
    metric?: Metric;
    max?: number;
    period?: string;
}

export interface Metric {
    id?: string;
    key?: string;
    displayName?: string;
}
