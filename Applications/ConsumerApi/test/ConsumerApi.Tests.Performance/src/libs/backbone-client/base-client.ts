import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { FluentClient } from "./fluent-client";
import { HttpClientConfiguration } from "./http-client-configuration";

export class BaseClient {
    protected readonly client: FluentClient;
    protected readonly httpxClient: Httpx;

    protected constructor(config: HttpClientConfiguration) {
        this.httpxClient = new Httpx({
            baseURL: config.baseUrl,
            timeout: config.timeout // 20s timeout
        });

        this.client = new FluentClient(this.httpxClient);
    }
}
