import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { PagedHttpResponseEnvelope } from 'src/app/utils/paged-http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class IdentityService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl + '/Identities';
    }

    getIdentities(
        pageNumber: number,
        pageSize: number
    ): Observable<PagedHttpResponseEnvelope<Identity>> {
        const httpOptions = {
            params: new HttpParams()
                .set('PageNumber', pageNumber + 1)
                .set('PageSize', pageSize),
        };

        return this.http.get<PagedHttpResponseEnvelope<Identity>>(
            this.apiUrl,
            httpOptions
        );
    }

    getIdentityByAddress(
        address: string
    ): Observable<HttpResponseEnvelope<Identity>> {
        // Missing an API endpoint to get an identity by Address
        const maxPageSize = 200;

        return new Observable<HttpResponseEnvelope<Identity>>((subscriber) => {
            this.getIdentities(0, maxPageSize).subscribe({
                next: (data: PagedHttpResponseEnvelope<Identity>) => {
                    const identity = data.result.find(
                        (t) => t.address == address
                    );

                    if (identity) {
                        subscriber.next({
                            result: identity,
                        } as HttpResponseEnvelope<Identity>);
                    } else {
                        subscriber.error({
                            message: `Identity with ID: ${address} could not be found.`,
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
}

export interface Identity {
    address: string;
    clientId: string;
    publicKey: string;
    createdAt: Date;
    identityVersion: string;
}
