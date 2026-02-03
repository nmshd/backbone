import axios, { AxiosResponse } from "axios";
import { HttpClientConfiguration } from "../backbone-client/http-client-configuration";
import { CreateChallengeResponse } from "../backbone-client/models";
import { CryptoHelper } from "./crypto-helper";

const config = new HttpClientConfiguration();
const client = axios.create({ baseURL: config.baseUrl, timeout: config.timeoutInMilliseconds });

async function main(): Promise<void> {
    for (let i = 0; i < 20; i++) {
        let promises = [];
        for (let j = 0; j < 10; j++) {
            promises.push(createIdentity(config.clientId, config.clientSecret, "Password123!"));
        }
        const results = await Promise.allSettled(promises);
        for (const res of results) {
            if (res.status === "fulfilled") {
                console.log(res.value.status);
            } else {
                console.error("Error creating identity:", res.reason);
            }
        }
    }
}

main().catch((e) => {
    console.error(e);
    throw e;
});

async function createIdentity(clientId: string, clientSecret: string, password: string): Promise<AxiosResponse> {
    try {
        const keyPair = await CryptoHelper.generateKeyPair();

        const challenge = await getChallenge();

        const signedChallenge = await CryptoHelper.signChallenge(keyPair, challenge);

        const createIdentityRequest: CreateIdentityRequest = {
            clientId: clientId,
            clientSecret: clientSecret,
            signedChallenge: { challenge: JSON.stringify(challenge), signature: b64encode(JSON.stringify(signedChallenge)) },
            identityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
            devicePassword: password,
            identityVersion: 1
        };

        const httpResponse = await client.post(`api/${config.apiVersion}/Identities`, JSON.stringify(createIdentityRequest), {
            headers: { "Content-Type": "application/json" }
        });

        console.log(JSON.stringify(httpResponse.status));

        return httpResponse;
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
