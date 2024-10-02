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
