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

    public post<T>(body: any | null, jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.post(this.endpoint, body, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "POST");
        return this.getJson<T>(response, jsonRootKey);
    }

    public put<T>(body: any | null, jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.put(this.endpoint, body, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "PUT");
        return this.getJson<T>(response, jsonRootKey);
    }

    public get<T>(jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.get(this.endpoint, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "GET");
        return this.getJson<T>(response, jsonRootKey);
    }

    public delete<T>(jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.delete(this.endpoint, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "DELETE");
        return this.getJson<T>(response, jsonRootKey);
    }

    public patch<T>(body: any, jsonRootKey: string | null = "result"): T {
        const response = this.statelessClient.httpxClient.patch(this.endpoint, body, { headers: this.headers });

        this.ThrowIfResponse4or5(response, "PATCH");
        return this.getJson<T>(response, jsonRootKey);
    }

    private getJson<T>(response: any, jsonRootKey: string | null): T {
        if (jsonRootKey === null) {
            return response.json() as T;
        }
        return response.json(jsonRootKey) as T;
    }

    private ThrowIfResponse4or5(response: any, type: string) {
        if (response.status.toString()[0] === "4" || response.status.toString()[0] === "5") {
            throw new Error(`Request ${type} ${this.endpoint} failed with status code ${response.status}: ${JSON.stringify(response.json())}`);
        }
    }
}
