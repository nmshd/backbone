import { JwtResponse } from ".";

export interface CreateIdentityRequest {
    clientId: string;
    clientSecret: string;
    identityPublicKey: any;
    devicePassword: string;
    identityVersion: number;
    signedChallenge: any;
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

export interface IdentityWithToken {
    response: CreateIdentityResponse;
    token: JwtResponse;
    password: string;
}
