import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { b64encode } from "k6/encoding";
import { Response } from "k6/http";
import { apiVersion } from ".";
import { CreateChallengeResponse, CreateIdentityRequest, JwtResponse } from "../../models";
import { ChallengeRequestPayload, CryptoHelper } from "../crypto-helper";

export function createIdentity(client: Httpx, clientId: string, clientSecret: string, password: string): Response {
    try {
        const keyPair = CryptoHelper.generateKeyPair();

        const challenge = getChallenge(client);

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

export function exchangeToken(client: Httpx, username: string, password: string): JwtResponse {
    const payload = {
        client_id: "test",
        client_secret: "test",
        grant_type: "password",
        username,
        password
    };
    return client
        .post("connect/token", payload, {
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            }
        })
        .json() as unknown as JwtResponse;
}

function getChallenge(client: Httpx): ChallengeRequestPayload {
    const receivedChallenge = client.post(`api/${apiVersion}/Challenges`).json("result") as CreateChallengeResponse;

    return {
        id: receivedChallenge.id,
        expiresAt: receivedChallenge.expiresAt
    };
}
