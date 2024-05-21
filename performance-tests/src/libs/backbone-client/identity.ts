import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { b64encode } from "k6/encoding";
import { Response } from "k6/http";
import { CreateChallengeResponse } from "../../models/challenge";
import { CreateIdentityRequest } from "../../models/identity";
import { JwtResponse } from "../../models/jwt-response";
import { ChallengeRequestPayload, CryptoHelper } from "../crypto-helper";

export function createIdentity(client: Httpx, clientId: string, clientSecret: string): { httpResponse: Response; generatedPassword: string } {
    try {
        const keyPair = CryptoHelper.generateKeyPair();

        const challenge = getChallenge(client);

        const signedChallenge = CryptoHelper.signChallenge(keyPair, challenge);

        const generatedPassword = CryptoHelper.generatePassword()!;

        const createIdentityRequest: CreateIdentityRequest = {
            clientId: clientId,
            clientSecret: clientSecret,
            signedChallenge: { challenge: JSON.stringify(challenge), signature: b64encode(JSON.stringify(signedChallenge)) },
            identityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
            devicePassword: generatedPassword,
            identityVersion: 1
        };

        const httpResponse = client.post("Identities", JSON.stringify(createIdentityRequest), {
            // eslint-disable-next-line @typescript-eslint/naming-convention
            headers: { "Content-Type": "application/json" }
        }) as Response;
        return { httpResponse, generatedPassword };
    } catch (e) {
        console.error(e);
        throw e;
    }
}

export function exchangeToken(client: Httpx, username: string, password: string): JwtResponse {
    const payload = {
        /* eslint-disable @typescript-eslint/naming-convention */
        client_id: "test",
        client_secret: "test",
        grant_type: "password",
        username,
        password
        /* eslint-enable @typescript-eslint/naming-convention */
    };
    return client
        .post("http://localhost:8081/connect/token", payload, {
            headers: {
                // eslint-disable-next-line @typescript-eslint/naming-convention
                "Content-Type": "application/x-www-form-urlencoded"
            }
        })
        .json() as unknown as JwtResponse;
}

function getChallenge(client: Httpx): ChallengeRequestPayload {
    const receivedChallenge = client.post("Challenges").json("result") as unknown as CreateChallengeResponse;

    return {
        id: receivedChallenge.id,
        expiresAt: receivedChallenge.expiresAt
    };
}
