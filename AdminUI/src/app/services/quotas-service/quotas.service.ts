import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class QuotasService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl;
    }

    getMetrics(): Observable<HttpResponseEnvelope<Metric>> {
        return this.http.get<HttpResponseEnvelope<Metric>>( this.apiUrl + "/Metrics");
    }

    getPeriods(): string[] {
        return ['Hourly', 'Daily', 'Weekly', 'Monthly', 'Yearly'];
    }

    createTierQuota(quota: Quota, tierId: string): Observable<HttpResponseEnvelope<Quota>> {
        return this.http.post<HttpResponseEnvelope<Quota>>(this.apiUrl + "/Tiers/" + tierId + "/Quotas", quota);
    }
}

export interface Quota {
    tierId?: string;
    metricKey?: string;
    max?: number;
    period?: string;
}

export interface Metric {
    id?: string;
    key?: string;
    displayName?: string;
}
