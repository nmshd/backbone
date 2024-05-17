import { Response } from "k6/http";
declare module "https://jslib.k6.io/httpx/0.1.0/index.js" {

    export class Httpx {
        constructor(options: Options);
        get(...args: any[]): Response;
        post(...args: any[]): Response;
        put(...args: any[]): Response;
    }

    export interface Options {
        baseURL: string;
        timeout: number;
        tags: string[];
    }
}