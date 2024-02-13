import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { AssignQuotaData } from "src/app/components/quotas/assign-quotas-dialog/assign-quotas-dialog.component";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class QuotasService {
    private readonly apiUrl: string;

    private readonly errorMessageSubject = new Subject<string>();
    public errorMessage$ = this.errorMessageSubject.asObservable();

    private readonly quotaSubject = new Subject<AssignQuotaData>();
    public quota$ = this.quotaSubject.asObservable();

    public constructor(private readonly http: HttpClient) {
        this.apiUrl = environment.apiUrl;
    }

    public sendErrorMessage(message: string): void {
        this.errorMessageSubject.next(message);
    }

    public passQuota(quota: AssignQuotaData): void {
        this.quotaSubject.next(quota);
    }

    public getMetrics(): Observable<HttpResponseEnvelope<Metric>> {
        return this.http.get<HttpResponseEnvelope<Metric>>(`${this.apiUrl}/Metrics`);
    }

    public getPeriods(): string[] {
        return ["Hour", "Day", "Week", "Month", "Year"];
    }

    public createTierQuota(request: CreateQuotaForTierRequest, tierId: string): Observable<HttpResponseEnvelope<TierQuota>> {
        return this.http.post<HttpResponseEnvelope<TierQuota>>(`${this.apiUrl}/Tiers/${tierId}/Quotas`, request);
    }

    public createIdentityQuota(request: CreateQuotaForIdentityRequest, identityAddress: string): Observable<HttpResponseEnvelope<IdentityQuota>> {
        return this.http.post<HttpResponseEnvelope<IdentityQuota>>(`${this.apiUrl}/Identities/${identityAddress}/Quotas`, request);
    }

    public deleteTierQuota(quotaId: string, tierId: string): Observable<any> {
        return this.http.delete<HttpResponseEnvelope<any>>(`${this.apiUrl}/Tiers/${tierId}/Quotas/${quotaId}`);
    }

    public deleteIdentityQuota(quotaId: string, identityAddress: string): Observable<any> {
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
    usage: number;
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
