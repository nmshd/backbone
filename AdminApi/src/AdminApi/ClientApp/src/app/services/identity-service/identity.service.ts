import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { NGXLogger } from "ngx-logger";
import { ODataFilterBuilder } from "odata-filter-builder";
import { Observable, map } from "rxjs";
import { DateFilter } from "src/app/utils/date-filter";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { NumberFilter } from "src/app/utils/number-filter";
import { ODataResponseEnvelope } from "src/app/utils/odata-response-envelope";
import { environment } from "src/environments/environment";
import { Quota } from "../quotas-service/quotas.service";
import { TierDTO } from "../tier-service/tier.service";

@Injectable({
    providedIn: "root"
})
export class IdentityService {
    private readonly apiUrl: string;
    private readonly odataUrl: string;

    public constructor(
        private readonly http: HttpClient,
        private readonly logger: NGXLogger
    ) {
        this.apiUrl = `${environment.apiUrl}/Identities`;
        this.odataUrl = `${environment.odataUrl}/Identities`;
    }

    public getIdentities(filter: IdentityOverviewFilter, pageNumber: number, pageSize: number): Observable<ODataResponseEnvelope<IdentityOverview[]>> {
        const paginationFilter = `$top=${pageSize}&$skip=${pageNumber * pageSize}&$count=true`;
        return this.http.get<any>(`${this.odataUrl}${this.buildODataFilter(filter, paginationFilter)}${this.buildODataExpand()}`).pipe(
            map((response) => {
                return {
                    result: response["value"],
                    count: response["@odata.count"]
                } as ODataResponseEnvelope<IdentityOverview[]>;
            })
        );
    }

    public getIdentityByAddress(address: string): Observable<HttpResponseEnvelope<Identity>> {
        return this.http.get<HttpResponseEnvelope<Identity>>(`${this.apiUrl}/${address}`);
    }

    private buildODataFilter(filter: IdentityOverviewFilter, paginationFilter: string): string {
        const odataFilter = ODataFilterBuilder();

        if (filter.address !== undefined && filter.address !== "") odataFilter.contains("address", filter.address);

        if (filter.tiers !== undefined && filter.tiers.length > 0) {
            const tiersFilter = new ODataFilterBuilder();
            filter.tiers.forEach((tier) => {
                tiersFilter.or((x) => x.eq("tier/Id", tier));
            });
            odataFilter.and((_) => tiersFilter);
        }

        if (filter.clients !== undefined && filter.clients.length > 0) {
            const clientsFilter = new ODataFilterBuilder();
            filter.clients.forEach((client) => {
                clientsFilter.or((x) => x.eq("createdWithClient", client));
            });
            odataFilter.and((_) => clientsFilter);
        }

        if (filter.createdAt.operator !== undefined && filter.createdAt.value !== undefined) {
            switch (filter.createdAt.operator) {
                case ">":
                    odataFilter.gt("createdAt", filter.createdAt.value.toISOString().slice(0, 10), false);
                    break;
                case "<":
                    odataFilter.lt("createdAt", filter.createdAt.value.toISOString().slice(0, 10), false);
                    break;
                case "=":
                    odataFilter.eq("createdAt", filter.createdAt.value.toISOString().slice(0, 10), false);
                    break;
                case "<=":
                    odataFilter.le("createdAt", filter.createdAt.value.toISOString().slice(0, 10), false);
                    break;
                case ">=":
                    odataFilter.ge("createdAt", filter.createdAt.value.toISOString().slice(0, 10), false);
                    break;
                default:
                    this.logger.error(`Invalid createdAt filter operator: ${filter.createdAt.operator}`);
                    break;
            }
        }

        if (filter.lastLoginAt.operator !== undefined && filter.lastLoginAt.value !== undefined) {
            switch (filter.lastLoginAt.operator) {
                case ">":
                    odataFilter.gt("lastLoginAt", filter.lastLoginAt.value.toISOString().slice(0, 10), false);
                    break;
                case "<":
                    odataFilter.lt("lastLoginAt", filter.lastLoginAt.value.toISOString().slice(0, 10), false);
                    break;
                case "=":
                    odataFilter.eq("lastLoginAt", filter.lastLoginAt.value.toISOString().slice(0, 10), false);
                    break;
                case "<=":
                    odataFilter.le("lastLoginAt", filter.lastLoginAt.value.toISOString().slice(0, 10), false);
                    break;
                case ">=":
                    odataFilter.ge("lastLoginAt", filter.lastLoginAt.value.toISOString().slice(0, 10), false);
                    break;
                default:
                    this.logger.error(`Invalid lastLoginAt filter operator: ${filter.lastLoginAt.operator}`);
                    break;
            }
        }

        if (filter.numberOfDevices.operator !== undefined && filter.numberOfDevices.value?.valueOf) {
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
                default:
                    this.logger.error(`Invalid numberOfDevices filter operator: ${filter.numberOfDevices.operator}`);
                    break;
            }
        }

        if (filter.datawalletVersion.operator !== undefined && filter.datawalletVersion.value?.valueOf) {
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
                default:
                    this.logger.error(`Invalid datawalletVersion filter operator: ${filter.datawalletVersion.operator}`);
                    break;
            }
        }

        if (filter.identityVersion.operator !== undefined && filter.identityVersion.value?.valueOf) {
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
                default:
                    this.logger.error(`Invalid identityVersion filter operator: ${filter.identityVersion.operator}`);
                    break;
            }
        }

        const filterComponents = [odataFilter.toString() !== "" ? `$filter=${odataFilter.toString()}` : "", paginationFilter];
        const filterParameter = `?${filterComponents.join("&")}`;

        return filterParameter;
    }

    private buildODataExpand(): string {
        const odataExpand = "&$expand=Tier";
        return odataExpand;
    }

    public updateIdentity(identity: Identity, params: UpdateTierRequest): Observable<HttpResponseEnvelope<Identity>> {
        return this.http.put<HttpResponseEnvelope<Identity>>(`${this.apiUrl}/${identity.address}`, params);
    }

    public getDeletionProcessOfIdentityById(address: String, deletionProcessId: String): Observable<HttpResponseEnvelope<DeletionProcess>> {
        return this.http.get<HttpResponseEnvelope<DeletionProcess>>(`${this.apiUrl}/${address}/DeletionProcesses/${deletionProcessId}`);
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
    tierId: string;
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
    tier: TierDTO;
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

export interface UpdateTierRequest {
    tierId: string;
}

export interface DeletionProcessAuditLog {
    id: string;
    createdAt: string;
    message: string;
    oldStatus: number;
    newStatus: number;
}

export interface DeletionProcess {
    id: string;
    status: string;
    createdAt: string;
    auditLog: DeletionProcessAuditLog[];
    approvalReminder1SentAt: Date;
    approvalReminder2SentAt: Date;
    approvalReminder3SentAt: Date;
    approvedAt: string;
    approvedByDevice: string;
    gracePeriodEndsAt: string;
    gracePeriodReminder1SentAt: Date;
    gracePeriodReminder2SentAt: Date;
    gracePeriodReminder3SentAt: Date;
    identityAddress: string;
}
