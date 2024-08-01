import { JwtResponse } from ".";
import { Identity } from "../libs/drept";

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

export interface IdentityWithToken extends Identity {
    response: CreateIdentityResponse;
    token: JwtResponse;
}
