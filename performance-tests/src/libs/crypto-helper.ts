import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";

export class CryptoHelper {
    public static readonly session = new Httpx({
        baseURL: "http://localhost:3000/",
        timeout: 2000,
        tags: ["k6-crypto"]
    });

    public static generateKeyPair(): KeyPair {
        return CryptoHelper.session.get("keypair").json() as unknown as KeyPair;
    }

    public static generatePassword(): string | undefined {
        return (CryptoHelper.session.get("password")).body?.toString();
    }

    public static signChallenge(keyPair: KeyPair, challenge: ChallengeRequestRepresentation): string | undefined {
        return CryptoHelper.session
            .post(
                "sign",
                JSON.stringify({
                    keyPair,
                    challenge: JSON.stringify(challenge)
                }),
                {
                    // eslint-disable-next-line @typescript-eslint/naming-convention
                    headers: { "Content-Type": "application/json" }
                }
            )
            .json();
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

export interface ChallengeRequestRepresentation {
    expiresAt: string;
    id: string;
    type: string;
}
