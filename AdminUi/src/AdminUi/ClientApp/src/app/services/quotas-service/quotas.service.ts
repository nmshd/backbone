import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class QuotasService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl;
    }

    getMetrics(): Observable<HttpResponseEnvelope<Metric>> {
        return this.http.get<HttpResponseEnvelope<Metric>>(this.apiUrl + "/Metrics");
    }

    getPeriods(): string[] {
        return ["Hour", "Day", "Week", "Month", "Year"];
    }

    createTierQuota(request: CreateQuotaForTierRequest, tierId: string): Observable<HttpResponseEnvelope<TierQuota>> {
        return this.http.post<HttpResponseEnvelope<TierQuota>>(`${this.apiUrl}/Tiers/${tierId}/Quotas`, request);
    }

    createIdentityQuota(request: CreateQuotaForIdentityRequest, identityAddress: string): Observable<HttpResponseEnvelope<IdentityQuota>> {
        return this.http.post<HttpResponseEnvelope<IdentityQuota>>(`${this.apiUrl}/Identities/${identityAddress}/Quotas`, request);
    }

    deleteTierQuota(quotaId: string, tierId: string): Observable<any> {
        return this.http.delete<HttpResponseEnvelope<any>>(`${this.apiUrl}/Tiers/${tierId}/Quotas/${quotaId}`);
    }

    deleteIdentityQuota(quotaId: string, identityAddress: string): Observable<any> {
        return this.http.delete<HttpResponseEnvelope<any>>(`${this.apiUrl}/Identities/${identityAddress}/Quotas/${quotaId}`);
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

export interface Quota {
    id: string;
    source: "Tier" | "Individual";
    metric: Metric;
    max: number;
    period: string;
    disabled: boolean;
    deleteable: boolean;
}

export interface CreateQuotaForTierRequest {
    metricKey: string;
    max: number;
    period: string;
}

export interface CreateQuotaForIdentityRequest {
    tierId?: string;
    metricKey: string;
    max: number;
    period: string;
}
