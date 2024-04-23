import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";

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
    group: "crypto"
});

export default async function () {
    const challenge = session.post("Challenges").json("result") as ChallengeResponse;
    const keyPair = cryptoSession.get("keypair").json();
    const signedChallenge = cryptoSession.post("sign", JSON.stringify({ keyPair, challenge }), { headers: { "Content-Type": "application/json" } }).json();

    const createIdentityRequest: CreateIdentityRequest = {
        ClientId: "test",
        ClientSecret: "test",
        SignedChallenge: signedChallenge,
        IdentityPublicKey: keyPair.publicKey,
        DevicePassword: "randomPassword",
        IdentityVersion: 1
    };
    const createdIdentity = session.post("Identities", JSON.stringify(createIdentityRequest));

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
