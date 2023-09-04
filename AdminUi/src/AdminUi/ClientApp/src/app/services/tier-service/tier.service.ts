import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";
import { TierQuota } from "../quotas-service/quotas.service";

@Injectable({
    providedIn: "root"
})
export class TierService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + "/Tiers";
    }

    getTiers(pageNumber?: number, pageSize?: number): Observable<PagedHttpResponseEnvelope<Tier>> {
        const httpOptions = {
            params: new HttpParams()
        };
        if (pageNumber) httpOptions.params.set("PageNumber", pageNumber + 1);
        if (pageSize) httpOptions.params.set("PageSize", pageSize);

        return this.http.get<PagedHttpResponseEnvelope<Tier>>(this.apiUrl, httpOptions);
    }

    getTierById(id: string): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.get<HttpResponseEnvelope<Tier>>(this.apiUrl + `/${id}`);
    }

    createTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.post<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }

    updateTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.put<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }

    deleteTierById(id: string): Observable<void> {
        return this.http.delete<void>(this.apiUrl + `/${id}`);
    }
}

export interface Tier {
    id: string;
    name: string;
    quotas: TierQuota[];
}
