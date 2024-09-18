import { b64encode } from "k6/encoding";
import { CryptoHelper } from "../crypto-helper";
import { BaseClient } from "./base-client";
import { apiVersion } from "./constants";
import { CreateChallengeResponse, CreateIdentityRequest, CreateIdentityResponse } from "./models";

export class UnauthenticatedClient extends BaseClient {
    public constructor() {
        super();
    }

    public createIdentity(clientId: string, clientSecret: string, password: string): CreateIdentityResponse {
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

            const httpResponse = this.httpxClient
                .post(`api/${apiVersion}/Identities`, JSON.stringify(createIdentityRequest), {
                    headers: { "Content-Type": "application/json" }
                })
                .json("result") as CreateIdentityResponse;
            return httpResponse;
        } catch (e) {
            console.error(e);
            throw e;
        }
    }

    public getChallenge(): CreateChallengeResponse {
        return this.httpxClient.post(`api/${apiVersion}/Challenges`).json("result") as CreateChallengeResponse;
    }
}
