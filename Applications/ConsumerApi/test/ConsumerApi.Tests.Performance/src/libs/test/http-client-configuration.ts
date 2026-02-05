export class HttpClientConfiguration {
    public get clientId(): string {
        return this.getEnvVar("clientId") ?? "test-eTn27eAkapU";
    }

    public get clientSecret(): string {
        return this.getEnvVar("clientSecret") ?? "ORmT192JehNLtVGYaGwCtesyspmGP6";
    }

    public get apiVersion(): string {
        return this.getEnvVar("apiVersion") ?? "v2";
    }

    public get baseUrl(): string {
        return this.getEnvVar("baseUrl") ?? "https://stage.enmeshed.eu/";
    }

    public get timeoutInMilliseconds(): number {
        return parseInt(this.getEnvVar("timeoutInMilliseconds") ?? "20000");
    }

    private getEnvVar(name: string): string | undefined {
        return process.env[name] as string | undefined;
    }
}
