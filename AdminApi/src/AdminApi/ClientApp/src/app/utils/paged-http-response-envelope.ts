import { PaginationData } from "./pagination-data";

export interface PagedHttpResponseEnvelope<Type> {
    result: Type[];
    pagination?: PaginationData;
}
