export class HttpClientConfiguration {
    public get baseUrl(): string {
        return this.getEnvVar("NMSHD_TEST_BASEURL") ?? "http://localhost:8081/";
    }

    public get clientId(): string {
        return this.getEnvVar("NMSHD_TEST_CLIENTID") ?? "test";
    }

    public get clientSecret(): string {
        return this.getEnvVar("NMSHD_TEST_CLIENTSECRET") ?? "test";
    }

    public get apiVersion(): string {
        return "v2";
    }

    private getEnvVar(name: string): string | undefined {
        return __ENV[name] as string | undefined;
    }
}
