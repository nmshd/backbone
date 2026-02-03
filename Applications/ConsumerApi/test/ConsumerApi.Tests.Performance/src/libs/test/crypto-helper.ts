import axios from "axios";

export class CryptoHelper {
    public static readonly client = axios.create({
        baseURL: "http://localhost:3000/",
        timeout: 2000
    });

    public static async generateKeyPair(): Promise<KeyPair> {
        return (await CryptoHelper.client.get<KeyPair>("keypair")).data;
    }

    public static async signChallenge(keyPair: KeyPair, challenge: ChallengeRequestPayload): Promise<CryptoSignature> {
        const response = await CryptoHelper.client.post<CryptoSignature>(
            "sign",
            JSON.stringify({
                keyPair,
                challenge: JSON.stringify(challenge)
            }),
            {
                headers: { "Content-Type": "application/json" }
            }
        );

        if (response.status !== 200) throw new Error(`Failed to sign challenge, status code: ${response.status}`);

        return response.data;
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

export interface CryptoSignature {
    signature: string;
    algorithm: string;
}
