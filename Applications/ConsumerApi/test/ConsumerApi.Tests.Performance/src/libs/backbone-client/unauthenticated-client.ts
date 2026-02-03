import { check } from "k6";
import { b64encode } from "k6/encoding";
import http from "k6/http"; // TODO: USE HTTPX AGAIN!!!!!
import { CryptoHelper } from "../crypto-helper";
import { HttpClientConfiguration } from "./http-client-configuration";
import { CreateChallengeResponse, CreateIdentityRequest } from "./models";

export class UnauthenticatedClient {
    public constructor(private readonly configuration: HttpClientConfiguration) {}

    public createIdentity(clientId: string, clientSecret: string, password: string): void {
        try {
            const keyPair = CryptoHelper.generateKeyPair();

            const challenge = this.getChallenge();

            const signedChallenge = CryptoHelper.signChallenge(keyPair, challenge);

            const createIdentityRequest: CreateIdentityRequest = {
                clientId: clientId,
                clientSecret: clientSecret,
                signedChallenge: {
                    challenge: JSON.stringify(challenge),
                    signature: b64encode(JSON.stringify(signedChallenge))
                },
                identityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
                devicePassword: password,
                identityVersion: 1
            };

            const httpResponse = http.post(`${this.configuration.baseUrl}api/${this.configuration.apiVersion}/Identities`, JSON.stringify(createIdentityRequest), {
                headers: { "Content-Type": "application/json" }
            });

            check(httpResponse, { "identity creation successful": (r) => r.status === 201 });

            console.log(JSON.stringify(httpResponse.status));
        } catch (e) {
            console.error(e);
            throw e;
        }
    }

    public getChallenge(): CreateChallengeResponse {
        const response = http.post(`${this.configuration.baseUrl}api/${this.configuration.apiVersion}/Challenges`);

        if (response.status !== 201) throw new Error(`Failed to get challenge, status code: ${response.status}`);

        return response.json("result") as any;
    }
}
