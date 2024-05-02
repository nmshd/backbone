import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";

export class Sidecar {
    private static instance: Sidecar | null = null;

    session = new Httpx({
        baseURL: "http://localhost:3000/",
        timeout: 2000,
        tags: ["k6-crypto"]
    });

    constructor() {
        if (Sidecar.instance === null) {
            Sidecar.instance = this;
        }
        return Sidecar.instance;
    }

    public GenerateKeyPair = (): KeyPair => this.session.get("keypair").json();

    public GeneratePassword = (): string => (this.session.get("password") as Response).body?.toString()!;

    public SignChallenge = (keyPair: KeyPair, challenge: ChallengeRequestRepresentation) => {
        return this.session
            .post(
                "sign",
                JSON.stringify({
                    keyPair,
                    challenge: JSON.stringify(challenge)
                }),
                {
                    headers: { "Content-Type": "application/json" }
                }
            )
            .json();
    };
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
