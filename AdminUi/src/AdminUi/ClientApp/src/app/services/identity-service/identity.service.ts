import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { PagedHttpResponseEnvelope } from "src/app/utils/paged-http-response-envelope";
import { environment } from "src/environments/environment";
import { Quota } from "../quotas-service/quotas.service";
import { ODataResponse } from "src/app/utils/odata-response";

@Injectable({
    providedIn: "root"
})
export class IdentityService {
    private readonly apiUrl: string;
    private readonly odataUrl: string;

    public constructor(private readonly http: HttpClient) {
        this.apiUrl = `${environment.apiUrl}/Identities`;
        this.odataUrl = `${environment.odataUrl}/Identities`;
    }

    public getIdentities(pageNumber: number, pageSize: number): Observable<ODataResponse<IdentityOverview[]>> {
        const httpOptions = {
            params: new HttpParams().set("PageNumber", pageNumber + 1).set("PageSize", pageSize)
        };

        return this.http.get<ODataResponse<IdentityOverview[]>>(this.odataUrl, httpOptions);
    }

    public getIdentityByAddress(address: string): Observable<HttpResponseEnvelope<Identity>> {
        return this.http.get<HttpResponseEnvelope<Identity>>(`${this.apiUrl}/${address}`);
    }
}

export interface Identity {
    address: string;
    clientId: string;
    publicKey: string;
    createdAt: Date;
    identityVersion: string;
    quotas: Quota[];
    devices: Device[];
}

export interface Device {
    id: string;
    username: string;
    createdAt: Date;
    lastLogin: LastLoginInformation;
    createdByDevice: string;
}

export interface LastLoginInformation {
    time?: Date;
}

export interface IdentityOverview {
    address: string;
    createdAt: Date;
    lastLoginAt: Date;
    createdWithClient: string;
    datawalletVersion: string;
    tierName: string;
    tierId: string;
    identityVersion: string;
    numberOfDevices: number;
}
