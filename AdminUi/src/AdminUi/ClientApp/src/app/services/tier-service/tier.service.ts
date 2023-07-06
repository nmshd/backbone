import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class TierService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + '/Tiers';
    }

    getTiers(
        pageNumber: number,
        pageSize: number
    ): Observable<PagedHttpResponseEnvelope<Tier>> {
        const httpOptions = {
            params: new HttpParams()
                .set('PageNumber', pageNumber + 1)
                .set('PageSize', pageSize),
        };

        return this.http.get<PagedHttpResponseEnvelope<Tier>>(
            this.apiUrl,
            httpOptions
        );
    }

    getTierById(id: string): Observable<HttpResponseEnvelope<Tier>> {
        // Missing an API endpoint to get a tier by ID
        const maxPageSize = 200;

        return new Observable<HttpResponseEnvelope<Tier>>((subscriber) => {
            this.getTiers(0, maxPageSize).subscribe({
                next: (data: PagedHttpResponseEnvelope<Tier>) => {
                    const tier = data.result.find((t) => t.id == id);

                    if (tier) {
                        subscriber.next({
                            result: tier,
                        } as HttpResponseEnvelope<Tier>);
                    } else {
                        subscriber.error({
                            message: `Tier with ID: ${id} could not be found.`,
                        });
                    }
                },
                complete: () => subscriber.complete(),
                error: (err: any) => {
                    subscriber.error(err);
                },
            });
        });
    }

    createTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.post<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }

    updateTier(tier: Tier): Observable<HttpResponseEnvelope<Tier>> {
        return this.http.put<HttpResponseEnvelope<Tier>>(this.apiUrl, tier);
    }
}

export interface Tier {
    id: string;
    name: string;
}
