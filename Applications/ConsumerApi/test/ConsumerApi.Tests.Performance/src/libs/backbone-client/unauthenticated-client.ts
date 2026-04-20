import { TypedResponse } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { b64encode } from "k6/encoding";
import { CryptoHelper } from "../crypto-helper";
import { BaseClient } from "./base-client";
import { HttpClientConfiguration } from "./http-client-configuration";
import { CreateChallengeResponse, CreateIdentityRequest } from "./models";

export class UnauthenticatedClient extends BaseClient {
    public constructor(configuration: HttpClientConfiguration) {
        super(configuration);
    }

    public createIdentity(password: string): TypedResponse<unknown> {
        try {
            const keyPair = CryptoHelper.generateKeyPair();

            const challenge = this.createChallenge();

            const signedChallenge = CryptoHelper.signChallenge(keyPair, challenge);

            const createIdentityRequest: CreateIdentityRequest = {
                clientId: this.configuration.clientId,
                clientSecret: this.configuration.clientSecret,
                signedChallenge: signedChallenge,
                identityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
                devicePassword: password,
                identityVersion: 1
            };

            const httpResponse = this.httpxClient.post(`${this.configuration.baseUrl}api/${this.configuration.apiVersion}/Identities`, JSON.stringify(createIdentityRequest), {
                headers: { "Content-Type": "application/json" }
            });

            return httpResponse;
        } catch (e) {
            console.error(e);
            throw e;
        }
    }

    public createChallenge(): CreateChallengeResponse {
        const response = this.httpxClient.post(`${this.configuration.baseUrl}api/${this.configuration.apiVersion}/Challenges`);

        if (response.status !== 201) throw new Error(`Failed to create challenge, status code: ${response.status}`);

        return response.json("result") as CreateChallengeResponse;
    }
}
