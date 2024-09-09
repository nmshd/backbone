// @ts-expect-error: k6 uses links to packages, which typescript cannot lint.
import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { defaultBaseUrl } from "./constants";
import { FluentClient } from "./fluent-client";

export class BaseEnmeshedClient {
    protected readonly clientId: string;
    protected readonly clientSecret: string;
    protected readonly client: FluentClient;
    protected readonly httpxClient: any;

    protected constructor() {
        this.clientId = (__ENV.clientId as string | undefined) ?? "test";
        this.clientSecret = (__ENV.clientSecret as string | undefined) ?? "test";
        this.httpxClient = new Httpx({
            baseURL: defaultBaseUrl,
            timeout: 20000 // 20s timeout
        });

        this.client = new FluentClient(this.httpxClient);
    }
}
