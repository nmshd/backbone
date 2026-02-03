import { check } from "k6";
import { b64encode } from "k6/encoding";
import { CryptoHelper } from "../crypto-helper";
import { BaseClient } from "./base-client";
import { HttpClientConfiguration } from "./http-client-configuration";
import { CreateChallengeResponse, CreateIdentityRequest } from "./models";

export class UnauthenticatedClient extends BaseClient {
    public constructor(configuration: HttpClientConfiguration) {
        super(configuration);
    }

    public createIdentity(clientId: string, clientSecret: string, password: string): void {
        try {
            const keyPair = CryptoHelper.generateKeyPair();

            const challenge = this.getChallenge();

            const signedChallenge = CryptoHelper.signChallenge(keyPair, challenge);

            const createIdentityRequest: CreateIdentityRequest = {
                clientId: clientId,
                clientSecret: clientSecret,
                signedChallenge: { challenge: JSON.stringify(challenge), signature: b64encode(JSON.stringify(signedChallenge)) },
                identityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
                devicePassword: password,
                identityVersion: 1
            };

            const httpResponse = this.httpxClient.post(`api/${this.configuration.apiVersion}/Identities`, JSON.stringify(createIdentityRequest), {
                headers: { "Content-Type": "application/json" }
            });

            check(httpResponse, { "identity creation successful": (r) => r.status === 201 });

            // return httpResponse.json("result") as CreateIdentityResponse;
        } catch (e) {
            console.error(e);
            throw e;
        }
    }

    public getChallenge(): CreateChallengeResponse {
        const response = this.httpxClient.post(`api/${this.configuration.apiVersion}/Challenges`);

        if (response.status !== 201) throw new Error(`Failed to get challenge, status code: ${response.status}`);

        return response.json("result") as CreateChallengeResponse;
    }
}
