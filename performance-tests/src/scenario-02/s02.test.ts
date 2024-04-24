import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { check } from "k6";
import { b64encode } from "k6/encoding";
import { Response } from "k6/http";

export const options = {
    scenarios: {
        constant_request_rate: {
            executor: "constant-arrival-rate",
            rate: 5,
            timeUnit: "1s",
            duration: "1m",
            preAllocatedVUs: 20,
            maxVUs: 100
        }
    }
};

const session = new Httpx({
    baseURL: "http://localhost:8081/api/v1/",
    timeout: 20000 // 20s timeout.
});

const cryptoSession = new Httpx({
    baseURL: "http://localhost:3000/",
    timeout: 2000,
    group: "crypto",
    tags: ["crypto"]
});

export default async function () {
    const createdIdentityResponse = CreateIdentity();

    check(createdIdentityResponse, {
        "Identity was created": (r) => r.status === 201,
        "response has Address": (r) => r.json("result.address") != undefined
    });
}

interface ChallengeResponse {
    id: string;
    expiresAt: string;
    createdBy: string;
    createdByDevice: string;
}

interface CreateIdentityRequest {
    ClientId: string;
    ClientSecret: string;
    IdentityPublicKey: any;
    DevicePassword: string;
    IdentityVersion: number;
    SignedChallenge: any;
}
function CreateIdentity() {
    const receivedChallenge = session.post("Challenges").json("result") as ChallengeResponse;
    const challenge = JSON.stringify({
        expiresAt: receivedChallenge.expiresAt,
        id: receivedChallenge.id,
        type: "Identity"
    });

    const keyPair = cryptoSession.get("keypair").json();

    const signedChallenge = SignChallenge(keyPair, challenge);

    const createIdentityRequest: CreateIdentityRequest = {
        ClientId: "test",
        ClientSecret: "test",
        SignedChallenge: { challenge, signature: b64encode(JSON.stringify(signedChallenge)) },
        IdentityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
        DevicePassword: "randomPassword",
        IdentityVersion: 1
    };
    const createdIdentityResponse = session.post("Identities", JSON.stringify(createIdentityRequest), { headers: { "Content-Type": "application/json" } }) as Response;
    return createdIdentityResponse;
}
function SignChallenge(keyPair: any, challenge: string) {
    return cryptoSession.post("sign", JSON.stringify({ keyPair, challenge }), { headers: { "Content-Type": "application/json" } }).json();
}
