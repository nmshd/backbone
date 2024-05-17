declare module "https://jslib.k6.io/httpx/0.1.0/index.js" {
    export class Httpx {
        constructor(options: Options);
        get(): unknown;
        post(): unknown;
        put(): unknown;
    }

    export interface Options {
        baseURL: string;
        timeout: number;
    }
}
