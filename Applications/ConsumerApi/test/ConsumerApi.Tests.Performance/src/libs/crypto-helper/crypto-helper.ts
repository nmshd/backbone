import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { b64encode } from "k6/encoding";

export class CryptoHelper {
    public static readonly client = new Httpx({
        baseURL: "http://localhost:3000/",
        timeout: 2000,
        tags: ["k6-crypto"]
    });

    public static generateKeyPair(): KeyPair {
        return CryptoHelper.client.get("keypair").json() as KeyPair;
    }

    public static signChallenge(keyPair: KeyPair, challenge: ChallengeRequestPayload): SignedChallenge {
        const stringifiedChallenge = JSON.stringify(challenge);
        const response = CryptoHelper.client.post(
            "sign",
            JSON.stringify({
                keyPair,
                challenge: stringifiedChallenge
            }),
            {
                headers: { "Content-Type": "application/json" }
            }
        );

        if (response.status !== 200) throw new Error(`Failed to sign challenge, status code: ${response.status}`);

        const signature = b64encode(JSON.stringify(response.json()));

        const result = { signature: signature, challenge: stringifiedChallenge };

        return result;
    }
}

interface KeyPair {
    pub: {
        pub: string;
        alg: number;
    };

    prv: {
        prv: string;
        alg: number;
    };
}

export interface ChallengeRequestPayload {
    id: string;
    expiresAt: string;
}

export interface SignedChallenge {
    challenge: string;
    signature: string;
}
