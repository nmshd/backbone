import { JwtResponse } from ".";
import { Identity } from "../libs/data-loader/models";

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

export interface IdentityWithAccessToken extends Identity {
    response: CreateIdentityResponse;
    token: JwtResponse;
    tokenExpiresAt: Date;
}
