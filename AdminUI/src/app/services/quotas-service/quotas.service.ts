import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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
        return this.http.get<HttpResponseEnvelope<Metric>>(
            this.apiUrl + '/Metrics'
        );
    }

    getPeriods(): string[] {
        return ['Hour', 'Day', 'Week', 'Month', 'Year'];
    }

    createTierQuota(
        request: CreateQuotaForTierRequest,
        tierId: string
    ): Observable<HttpResponseEnvelope<TierQuota>> {
        return this.http.post<HttpResponseEnvelope<TierQuota>>(
            this.apiUrl + '/Tiers/' + tierId + '/Quotas',
            request
        );
    }

    createIdentityQuota(
        request: CreateQuotaForIdentityRequest,
        identityAddress: string
    ): Observable<HttpResponseEnvelope<IdentityQuota>> {
        return this.http.post<HttpResponseEnvelope<IdentityQuota>>(
            this.apiUrl + '/Identity/' + identityAddress + '/Quotas',
            request
        );
    }
}

export interface Metric {
    key: string;
    displayName: string;
}

export interface TierQuota {
    id: string;
    metric: Metric;
    max: number;
    period: string;
}

export interface IdentityQuota {
    id: string;
    metric: Metric;
    max: number;
    period: string;
}

export interface CreateQuotaForTierRequest {
    metricKey: string;
    max: number;
    period: string;
}

export interface CreateQuotaForIdentityRequest {
    metricKey: string;
    max: number;
    period: string;
}
