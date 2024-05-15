import { b64encode } from "k6/encoding";
import { Response } from "k6/http";
import { ChallengeResponse } from "../../domain/challenge";
import { CreateIdentityRequest, CreateIdentityResponse } from "../../domain/identity";
import { TokenResponse } from "../../domain/token";
import { ChallengeRequestRepresentation, CryptoHelper } from "../crypto-helper";
import { HttpxClient } from "../k6-utils";

export function CreateIdentity(client: HttpxClient, ClientId: string, ClientSecret: string): { httpResponse: Response; generatedPassword: string } {
    const sidecar = new CryptoHelper();

    try {
        const challenge = getChallenge(client);

        const keyPair = sidecar.GenerateKeyPair();

        const signedChallenge = sidecar.SignChallenge(keyPair, challenge);

        const generatedPassword = sidecar.GeneratePassword();

        const createIdentityRequest: CreateIdentityRequest = {
            ClientId,
            ClientSecret,
            SignedChallenge: { challenge: JSON.stringify(challenge), signature: b64encode(JSON.stringify(signedChallenge)) },
            IdentityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
            DevicePassword: generatedPassword,
            IdentityVersion: 1
        };

        const httpResponse = client.post("Identities", JSON.stringify(createIdentityRequest), { headers: { "Content-Type": "application/json" } }) as Response;
        return { httpResponse, generatedPassword };
    } catch (e) {
        console.error(e);
        throw e;
    }
}

export function ExchangeToken(client: HttpxClient, createdIdentityResponse: CreateIdentityResponse, password: string) {
    const payload = {
        client_id: "test",
        client_secret: "test",
        grant_type: "password",
        username: createdIdentityResponse.device.username,
        password
    };
    return client.post("http://localhost:8081/connect/token", payload, { headers: { "Content-Type": "application/x-www-form-urlencoded" } }).json() as TokenResponse;
}

function getChallenge(client: HttpxClient): ChallengeRequestRepresentation {
    const receivedChallenge = client.post("Challenges").json("result") as ChallengeResponse;

    return {
        expiresAt: receivedChallenge.expiresAt,
        id: receivedChallenge.id,
        type: "Identity"
    };
}
