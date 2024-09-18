import { TypedResponse } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { RequestBody } from "k6/http";
import { FluentClient } from "./fluent-client";

export class FluentRequest {
    private readonly statelessClient: FluentClient;
    private endpoint!: string;
    private headers!: Record<string, string | number | string[]>;

    public constructor(client: FluentClient) {
        this.statelessClient = client;
    }

    public setEndpoint(endpoint: string): this {
        this.endpoint = endpoint;
        return this;
    }

    public setHeaders(headers: Record<string, string | number | string[]>): this {
        this.headers = headers;
        return this;
    }

    public authenticate(token: string): this {
        this.headers = { ...this.headers, Authorization: token };
        return this;
    }

    public withJsonBody(): this {
        this.headers = { ...this.headers, "Content-Type": "application/json" };
        return this;
    }

    public post<T>(body: RequestBody | null, jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.post<T>(this.endpoint, body, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "POST");
        return this.getJson<T>(response, jsonRootKey);
    }

    public put<T>(body: RequestBody, jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.put<T>(this.endpoint, body, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "PUT");
        return this.getJson<T>(response, jsonRootKey);
    }

    public get<T>(jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.get<T>(this.endpoint, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "GET");
        return this.getJson<T>(response, jsonRootKey);
    }

    public delete<T>(jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.delete<T>(this.endpoint, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "DELETE");
        return this.getJson<T>(response, jsonRootKey);
    }

    public patch<T>(body: RequestBody, jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.patch<T>(this.endpoint, body, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "PATCH");
        return this.getJson<T>(response, jsonRootKey);
    }

    private getJson<RT>(response: TypedResponse<RT>, jsonRootKey: string | null) {
        if (jsonRootKey === null) {
            return response.json();
        }
        return response.json(jsonRootKey);
    }

    private ThrowIfResponse4or5<T>(response: TypedResponse<T>, type: string) {
        if (response.status > 399) {
            throw new Error(`Request ${type} ${this.endpoint} failed with status code ${response.status}: ${JSON.stringify(response.json())}`);
        }
    }
}
