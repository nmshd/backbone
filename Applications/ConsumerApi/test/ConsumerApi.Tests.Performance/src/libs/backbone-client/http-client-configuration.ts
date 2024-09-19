export class HttpClientConfiguration {
    private _clientId?: string;
    private _clientSecret?: string;
    private _apiVersion = "v1";
    private _baseUrl = "http://localhost:8081/";
    /**
     * in milliseconds
     */
    private _timeout = 20000;

    public get clientId(): string {
        return this._clientId ?? (__ENV.clientId as string | undefined) ?? "test";
    }

    public set clientId(value: string) {
        this._clientId = value;
    }

    public get clientSecret(): string {
        return this._clientSecret ?? (__ENV.clientSecret as string | undefined) ?? "test";
    }

    public set clientSecret(value: string) {
        this._clientSecret = value;
    }

    public get apiVersion(): string {
        return this._apiVersion;
    }

    public set apiVersion(value: string) {
        this._apiVersion = value;
    }

    public get baseUrl(): string {
        return this._baseUrl;
    }

    public set baseUrl(value: string) {
        this._baseUrl = value;
    }

    public get timeout(): number {
        return this._timeout;
    }

    public set timeout(value: number) {
        this._timeout = value;
    }
}
