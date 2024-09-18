declare module "https://jslib.k6.io/httpx/0.1.0/index.js" {
    export class Httpx {
        constructor(options: Options);

        get<RT>(url: string | HttpURL, params?: RefinedParams<RT> | null): TypedResponse<RT>;

        delete<RT>(url: string | HttpURL, params?: RefinedParams<RT> | null): TypedResponse<RT>;

        post<RT>(url: string | HttpURL, body?: RequestBody | null, params?: RefinedParams<RT> | null): TypedResponse<RT>;

        put<RT>(url: string | HttpURL, body?: RequestBody | null, params?: RefinedParams<RT> | null): TypedResponse<RT>;

        patch<RT>(url: string | HttpURL, body?: RequestBody | null, params?: RefinedParams<RT> | null): TypedResponse<RT>;
    }

    export class TypedResponse<RT> extends Response {
        json(path?: string): RT;
    }

    export interface Options {
        baseURL: string;
        timeout: number;
        tags?: string[];
    }
}
