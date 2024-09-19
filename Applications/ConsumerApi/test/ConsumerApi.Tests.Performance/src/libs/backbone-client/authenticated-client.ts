import { BaseClient } from "./base-client";
import { HttpClientConfiguration } from "./http-client-configuration";
import { CreateChallengeResponse, JwtResponse } from "./models";

export class AuthenticatedClient extends BaseClient {
    private token_expires_at = new Date();
    private access_token = "";
    private readonly configuration: HttpClientConfiguration;
    private readonly password: string;
    private readonly username: string;

    public constructor(username: string, password: string, configuration: HttpClientConfiguration | null = null) {
        configuration = configuration ?? new HttpClientConfiguration();

        super(configuration);
        this.configuration = configuration;

        this.username = username;
        this.password = password;
    }

    private tokenIsStillValid(): boolean {
        return +this.token_expires_at - 1000 > +new Date();
    }

    private exchangeToken() {
        const payload = {
            client_id: this.configuration.clientId,
            client_secret: this.configuration.clientSecret,
            grant_type: "password",
            username: this.username,
            password: this.password
        };
        const response = this.client.request().setEndpoint("connect/token").setHeaders({ "Content-Type": "application/x-www-form-urlencoded" }).post<JwtResponse>(payload, null);

        this.access_token = `${response.token_type} ${response.access_token}`;
        this.token_expires_at = new Date(+new Date() + response.expires_in * 1000);
    }

    private getAccessToken(): string {
        if (!this.tokenIsStillValid()) {
            this.exchangeToken();
        }

        return this.access_token;
    }

    public getChallenge(): CreateChallengeResponse {
        return this.client.request().setEndpoint(`api/${this.configuration.apiVersion}/Challenges`).withJsonBody().authenticate(this.getAccessToken()).post<CreateChallengeResponse>(null);
    }
}
