import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { b64encode } from "k6/encoding";

export const options = {
    iterations: 1
};

// const enmeshedOptions = {
//     platformClientId: "test",
//     platformClientSecret: "test"
// };

const session = new Httpx({
    baseURL: "http://localhost:8081/api/v1/",
    headers: {
        // "User-Agent": "My custom user agent",
        // "Content-Type": "application/x-www-form-urlencoded"
    },
    timeout: 20000 // 20s timeout.
});

const cryptoSession = new Httpx({
    baseURL: "http://localhost:3000/",
    timeout: 2000,
    group: "crypto",
    tags: ["crypto"]
});

export default async function () {
    const receivedChallenge = session.post("Challenges").json("result") as ChallengeResponse;
    const challenge = JSON.stringify({
        expiresAt: receivedChallenge.expiresAt,
        id: receivedChallenge.id,
        type: "Identity"
    });
    const keyPair = cryptoSession.get("keypair").json();
    const signedChallenge = cryptoSession.post("sign", JSON.stringify({ keyPair, challenge }), { headers: { "Content-Type": "application/json" } }).json();

    const createIdentityRequest: CreateIdentityRequest = {
        ClientId: "test",
        ClientSecret: "test",
        SignedChallenge: { challenge, signature: b64encode(JSON.stringify(signedChallenge)) },
        IdentityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
        DevicePassword: "randomPassword",
        IdentityVersion: 1
    };
    console.error(createIdentityRequest);
    const createdIdentity = session.post("Identities", JSON.stringify(createIdentityRequest), { headers: { "Content-Type": "application/json" } });

    console.log(createdIdentity);
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
