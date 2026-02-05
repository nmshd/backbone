import { HttpClientConfiguration } from "../libs/backbone-client/http-client-configuration";

export class Configuration {
    private constructor() {
        // hide constructor to enforce use of static `load` function
    }

    public get snapshot(): string {
        return this.getEnvVar("snapshot") ?? "light";
    }

    private getEnvVar(name: string): string | undefined {
        return __ENV[name] as string | undefined;
    }

    public get httpClient(): HttpClientConfiguration {
        return new HttpClientConfiguration();
    }

    public static load(): Configuration {
        return new Configuration();
    }
}
