declare module "https://jslib.k6.io/httpx/0.1.0/index.js" {
    export class Httpx {
        constructor(options: Options);
        get(...args: any[]): any;
        post(...args: any[]): any;
        put(...args: any[]): any;
    }

    export interface Options {
        baseURL: string;
        timeout: number;
        tags: string[];
    }
}
