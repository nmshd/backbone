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

    getMetrics(): Observable<HttpResponseEnvelope<MetricDTO>> {
        return this.http.get<HttpResponseEnvelope<MetricDTO>>(
            this.apiUrl + '/Metrics'
        );
    }

    getPeriods(): string[] {
        return ['Hour', 'Day', 'Week', 'Month', 'Year'];
    }

    createTierQuota(
        request: CreateQuotaForTierRequest,
        tierId: string
    ): Observable<HttpResponseEnvelope<TierQuotaDefinitionDTO>> {
        return this.http.post<HttpResponseEnvelope<TierQuotaDefinitionDTO>>(
            this.apiUrl + '/Tiers/' + tierId + '/Quotas',
            request
        );
    }

    createIdentityQuota(
        request: IdentityQuotaDefinitionDTO,
        identityAddress: string
    ): Observable<HttpResponseEnvelope<CreateQuotaForIdentityRequest>> {
        return this.http.post<
            HttpResponseEnvelope<CreateQuotaForIdentityRequest>
        >(this.apiUrl + '/Identity/' + identityAddress + '/Quotas', request);
    }
}

export interface MetricDTO {
    key: string;
    displayName: string;
}

export interface TierQuotaDefinitionDTO {
    id: string;
    metric: MetricDTO;
    max: number;
    period: string;
}

export interface IdentityQuotaDefinitionDTO {
    id: string;
    metric: MetricDTO;
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
