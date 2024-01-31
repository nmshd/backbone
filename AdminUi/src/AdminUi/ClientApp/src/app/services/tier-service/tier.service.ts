import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";
import { TierQuota } from "../quotas-service/quotas.service";

@Injectable({
    providedIn: "root"
})
export class TierService {
    private readonly apiUrl: string;
    private readonly refreshDataSubject = new BehaviorSubject<boolean>(false);
    public refreshData$ = this.refreshDataSubject.asObservable();

    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Tiers`;
    }

    public triggerRefresh(): void {
        this.refreshDataSubject.next(true);
    }

    public getTiers(): Observable<PagedHttpResponseEnvelope<TierOverview>> {
        return this.http.get<PagedHttpResponseEnvelope<TierOverview>>(this.apiUrl);
    }

    public getTierById(id: string): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.get<HttpResponseEnvelope<Tier>>(`${this.apiUrl}/${id}`);
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
