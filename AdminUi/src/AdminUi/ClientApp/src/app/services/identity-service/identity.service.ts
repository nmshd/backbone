import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { environment } from "src/environments/environment";
import { Quota } from "../quotas-service/quotas.service";
import { ODataResponse } from "src/app/utils/odata-response";
import ODataFilterBuilder from "odata-filter-builder";
import { NumberFilter } from "src/app/utils/number-filter";
import { DateFilter } from "src/app/utils/date-filter";

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

    public getIdentities(filter: IdentityOverviewFilter, pageNumber: number, pageSize: number): Observable<ODataResponse<IdentityOverview[]>> {
        return this.http.get<ODataResponse<IdentityOverview[]>>(`${this.odataUrl}${this.buildODataFilter(filter)}`);
    }

    public getIdentityByAddress(address: string): Observable<HttpResponseEnvelope<Identity>> {
        return this.http.get<HttpResponseEnvelope<Identity>>(`${this.apiUrl}/${address}`);
    }

    private buildODataFilter(filter: IdentityOverviewFilter): string {
        var odataFilter = ODataFilterBuilder();

        if (filter.address != null && filter.address != "") odataFilter.contains("address", filter.address);

        if (filter.tiers != null && filter.tiers.length > 0) {
            filter.tiers.forEach((tier) => {
                odataFilter.eq("tierId", tier);
            });
        }

        if (filter.clients != null && filter.clients.length > 0) {
            filter.clients.forEach((client) => {
                odataFilter.eq("createdWithClient", client);
            });
        }

        if (filter.createdAt.operator != null && filter.createdAt.value != null) {
            switch (filter.createdAt.operator) {
                case ">":
                    odataFilter.gt("createdAt", filter.createdAt.value);
                    break;
                case "<":
                    odataFilter.lt("createdAt", filter.createdAt.value);
                    break;
                case "=":
                    odataFilter.eq("createdAt", filter.createdAt.value);
                    break;
                case "<=":
                    odataFilter.le("createdAt", filter.createdAt.value);
                    break;
                case ">=":
                    odataFilter.ge("createdAt", filter.createdAt.value);
                    break;
            }
        }

        if (filter.lastLoginAt.operator != null && filter.lastLoginAt.value != null) {
            switch (filter.lastLoginAt.operator) {
                case ">":
                    odataFilter.gt("lastLoginAt", filter.lastLoginAt.value);
                    break;
                case "<":
                    odataFilter.lt("lastLoginAt", filter.lastLoginAt.value);
                    break;
                case "=":
                    odataFilter.eq("lastLoginAt", filter.lastLoginAt.value);
                    break;
                case "<=":
                    odataFilter.le("lastLoginAt", filter.lastLoginAt.value);
                    break;
                case ">=":
                    odataFilter.ge("lastLoginAt", filter.lastLoginAt.value);
                    break;
            }
        }

        if (filter.numberOfDevices.operator != null && filter.numberOfDevices.value != null) {
            switch (filter.numberOfDevices.operator) {
                case ">":
                    odataFilter.gt("numberOfDevices", filter.numberOfDevices.value);
                    break;
                case "<":
                    odataFilter.lt("numberOfDevices", filter.numberOfDevices.value);
                    break;
                case "=":
                    odataFilter.eq("numberOfDevices", filter.numberOfDevices.value);
                    break;
                case "<=":
                    odataFilter.le("numberOfDevices", filter.numberOfDevices.value);
                    break;
                case ">=":
                    odataFilter.ge("numberOfDevices", filter.numberOfDevices.value);
                    break;
            }
        }

        if (filter.datawalletVersion.operator != null && filter.datawalletVersion.value != null) {
            switch (filter.datawalletVersion.operator) {
                case ">":
                    odataFilter.gt("datawalletVersion", filter.datawalletVersion.value);
                    break;
                case "<":
                    odataFilter.lt("datawalletVersion", filter.datawalletVersion.value);
                    break;
                case "=":
                    odataFilter.eq("datawalletVersion", filter.datawalletVersion.value);
                    break;
                case "<=":
                    odataFilter.le("datawalletVersion", filter.datawalletVersion.value);
                    break;
                case ">=":
                    odataFilter.ge("datawalletVersion", filter.datawalletVersion.value);
                    break;
            }
        }

        if (filter.identityVersion.operator != null && filter.identityVersion.value != null) {
            switch (filter.identityVersion.operator) {
                case ">":
                    odataFilter.gt("identityVersion", filter.identityVersion.value);
                    break;
                case "<":
                    odataFilter.lt("identityVersion", filter.identityVersion.value);
                    break;
                case "=":
                    odataFilter.eq("identityVersion", filter.identityVersion.value);
                    break;
                case "<=":
                    odataFilter.le("identityVersion", filter.identityVersion.value);
                    break;
                case ">=":
                    odataFilter.ge("identityVersion", filter.identityVersion.value);
                    break;
            }
        }

        let filterParameter = "";
        if (odataFilter.toString() != "") filterParameter = `?$filter=${odataFilter.toString()}`;

        return filterParameter;
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

export interface IdentityOverviewFilter {
    address?: string;
    tiers?: string[];
    clients?: string[];
    numberOfDevices: NumberFilter;
    createdAt: DateFilter;
    lastLoginAt: DateFilter;
    datawalletVersion: NumberFilter;
    identityVersion: NumberFilter;
}
