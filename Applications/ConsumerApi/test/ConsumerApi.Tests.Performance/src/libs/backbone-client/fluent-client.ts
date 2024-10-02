import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { FluentRequest } from "./fluent-request";

export class FluentClient {
    public readonly httpxClient: Httpx;

    public constructor(client: Httpx) {
        this.httpxClient = client;
    }

    public request(): FluentRequest {
        return new FluentRequest(this);
    }
}
