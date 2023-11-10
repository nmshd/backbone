import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class RelationshipService {
    private readonly apiUrl: string;

    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Relationships`;
    }

    public getRelationshipsByParticipantAddress(address: string, pageNumber: number, pageSize: number): Observable<PagedHttpResponseEnvelope<Relationship>> {
        const httpOptions = {
            params: new HttpParams()
                .set("Participant", address)
                .set("PageNumber", pageNumber + 1)
                .set("PageSize", pageSize)
        };

        return this.http.get<PagedHttpResponseEnvelope<Relationship>>(this.apiUrl, httpOptions);
    }
}

export interface Relationship {
    peer: string;
    requestedBy: string;
    templateId: string;
    status: string;
    creationDate: Date;
    answeredAt?: Date;
    createdByDevice: string;
    answeredByDevice: string;
}
