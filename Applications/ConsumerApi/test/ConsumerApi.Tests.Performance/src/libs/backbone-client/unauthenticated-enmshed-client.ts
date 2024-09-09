// @ts-expect-error: k6 uses links to packages, which typescript cannot lint.
import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { b64encode } from "k6/encoding";
import { Response } from "k6/http";
import { apiVersion } from ".";
import { CreateChallengeResponse, CreateIdentityRequest } from "../../models";
import { ChallengeRequestPayload, CryptoHelper } from "../crypto-helper";
import { BaseEnmeshedClient } from "./base-enmshed-client";

export class UnauthenticatedEnmeshedClient extends BaseEnmeshedClient {
    public constructor() {
        super();
    }

    public createIdentity(client: Httpx, clientId: string, clientSecret: string, password: string): Response {
        try {
            const keyPair = CryptoHelper.generateKeyPair();

            const challenge = this.getChallenge(client);

            const signedChallenge = CryptoHelper.signChallenge(keyPair, challenge);

            const createIdentityRequest: CreateIdentityRequest = {
                clientId: clientId,
                clientSecret: clientSecret,
                signedChallenge: { challenge: JSON.stringify(challenge), signature: b64encode(JSON.stringify(signedChallenge)) },
                identityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
                devicePassword: password,
                identityVersion: 1
            };

            const httpResponse = client.post(`api/${apiVersion}/Identities`, JSON.stringify(createIdentityRequest), {
                headers: { "Content-Type": "application/json" }
            }) as Response;
            return httpResponse;
        } catch (e) {
            console.error(e);
            throw e;
        }
    }

    public getChallenge(client: Httpx): ChallengeRequestPayload {
        const receivedChallenge = client.post(`api/${apiVersion}/Challenges`).json("result") as CreateChallengeResponse;

        return {
            id: receivedChallenge.id,
            expiresAt: receivedChallenge.expiresAt
        };
    }
}
