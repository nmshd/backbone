declare module "https://jslib.k6.io/httpx/0.1.0/index.js" {
    export class Httpx {
        constructor(options: Options);

        get<RT extends ResponseType | undefined>(
            url: string | HttpURL,
            params?: RefinedParams<RT> | null,
        ): RefinedResponse<RT>;

        post<RT extends ResponseType | undefined>(
            url: string | HttpURL,
            body?: RequestBody | null,
            params?: RefinedParams<RT> | null,
        ): RefinedResponse<RT>;

        put<RT extends ResponseType | undefined>(
            url: string | HttpURL,
            body?: RequestBody | null,
            params?: RefinedParams<RT> | null,
        ): RefinedResponse<RT>;
    }

    export class RefinedResponse<RT> {
        json(path?: string): any;
    }

    export interface Options {
        baseURL: string;
        timeout: number;
        tags?: string[];
    }
}
