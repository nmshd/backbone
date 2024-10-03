export class HttpClientConfiguration {
    public get clientId(): string {
        return this.getEnvVar("clientId") ?? "test";
    }

    public get clientSecret(): string {
        return this.getEnvVar("clientSecret") ?? "test";
    }

    public get apiVersion(): string {
        return this.getEnvVar("apiVersion") ?? "v1";
    }

    public get baseUrl(): string {
        return this.getEnvVar("baseUrl") ?? "http://localhost:8081/";
    }

    public get timeoutInMilliseconds(): number {
        return parseInt(this.getEnvVar("timeoutInMilliseconds") ?? "20000");
    }

    private getEnvVar(name: string): string | undefined {
        return __ENV[name] as string | undefined;
    }
}
