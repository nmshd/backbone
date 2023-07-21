import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { environment } from "src/environments/environment";
import { Metric } from "../metrics-service/metrics.service";

@Injectable({
    providedIn: "root"
})
export class QuotasService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl;
    }

    getPeriods(): string[] {
        return ["Hour", "Day", "Week", "Month", "Year"];
    }

    createTierQuota(quota: TierQuota, tierId: string): Observable<HttpResponseEnvelope<TierQuota>> {
        return this.http.post<HttpResponseEnvelope<TierQuota>>(`${this.apiUrl}/Tiers/${tierId}/Quotas`, quota);
    }

    deleteTierQuota(quotaId: string, tierId: string): Observable<any> {
        return this.http.delete<HttpResponseEnvelope<any>>(`${this.apiUrl}/Tiers/${tierId}/Quotas/${quotaId}`);
    }
}

export interface TierQuota {
    id: string;
    metric: Metric;
    max: number;
    period: string;
}

export interface Quota {
    tierId?: string;
    metricKey: string;
    max: number;
    period: string;
}
