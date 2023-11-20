import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, map } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";
import { TierQuota } from "../quotas-service/quotas.service";

@Injectable({
    providedIn: "root"
})
export class TierService {
    private readonly apiUrl: string;
    private static readonly QUEUED_FOR_DELETION_TIER_ID = "TIR00000000000000001";
    private static readonly BASIC_TIER_NAME = "Basic";

    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Tiers`;
    }

    public getTiers(): Observable<PagedHttpResponseEnvelope<TierOverview>> {
        return this.http.get<PagedHttpResponseEnvelope<TierOverview>>(this.apiUrl);
    }

    public getTierById(id: string): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.get<HttpResponseEnvelope<Tier>>(`${this.apiUrl}/${id}`).pipe(
            map((responseEnvelope: HttpResponseEnvelope<Tier>) => {
                responseEnvelope.result.isDeletable = responseEnvelope.result.name !== TierService.BASIC_TIER_NAME && responseEnvelope.result.id !== TierService.QUEUED_FOR_DELETION_TIER_ID;
                responseEnvelope.result.isReadOnly = responseEnvelope.result.id === TierService.QUEUED_FOR_DELETION_TIER_ID;
                return responseEnvelope;
            })
        );
    }

    public createTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.post<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }

    public updateTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.put<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }

    public deleteTierById(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}

export interface Tier {
    id: string;
    name: string;
    quotas: TierQuota[];
    isDeletable: boolean;
    isReadOnly: boolean;
}

export interface TierOverview {
    id: string;
    name: string;
    numberOfIdentities: number;
}

export interface TierDTO {
    id: string;
    name: string;
}
