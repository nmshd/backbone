import axios from "axios";
import { CreateChallengeResponse } from "../backbone-client/models";
import { CryptoHelper } from "./crypto-helper";
import { HttpClientConfiguration } from "./http-client-configuration";

const config = new HttpClientConfiguration();
const client = axios.create({ baseURL: config.baseUrl, timeout: config.timeoutInMilliseconds });

async function main(): Promise<void> {
    for (let i = 0; i < 2; i++) {
        let promises = [];
        for (let j = 0; j < 5; j++) {
            promises.push(createIdentity("Password123!"));
        }
        await Promise.allSettled(promises);
    }
}

main().catch((e) => {
    console.error(e);
    throw e;
});

async function createIdentity(password: string) {
    try {
        const keyPair = await CryptoHelper.generateKeyPair();

        const challenge = await getChallenge();

        const signedChallenge = await CryptoHelper.signChallenge(keyPair, challenge);

        const challengeForJson = JSON.stringify(challenge);
        const signatureForJson = b64encode(JSON.stringify(signedChallenge));
        const publicKeyForJson = b64encode(JSON.stringify(keyPair.pub));

        console.log(`challenge: ${challengeForJson}`);
        console.log(`signature: ${signatureForJson}`);
        console.log(`public key: ${publicKeyForJson}`);

        const createIdentityRequest: CreateIdentityRequest = {
            clientId: config.clientId,
            clientSecret: config.clientSecret,
            signedChallenge: {
                challenge: challengeForJson,
                signature: signatureForJson
            },
            identityPublicKey: publicKeyForJson,
            devicePassword: password,
            identityVersion: 1
        };

        const httpResponse = await client.post(`api/${config.apiVersion}/Identities`, JSON.stringify(createIdentityRequest), {
            headers: { "Content-Type": "application/json" }
        });

        console.log(JSON.stringify(httpResponse.status));
    } catch (e) {
        console.error(e);
        throw e;
    }
}

async function getChallenge(): Promise<CreateChallengeResponse> {
    const response = await client.post(`api/${config.apiVersion}/Challenges`);

    if (response.status !== 201) throw new Error(`Failed to get challenge, status code: ${response.status}`);

    return response.data.result as CreateChallengeResponse;
}

function b64encode(input: string): string {
    return Buffer.from(input).toString("base64");
}
export interface CreateIdentityRequest {
    clientId: string;
    clientSecret: string;
    identityPublicKey: unknown;
    devicePassword: string;
    identityVersion: number;
    signedChallenge: unknown;
}

export interface CreateIdentityResponse {
    address: string;
    createdAt: string;
    device: {
        createdAt: string;
        id: string;
        username: string;
    };
}
