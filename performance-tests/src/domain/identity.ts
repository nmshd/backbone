import { TokenResponse } from "./token";

export interface CreateIdentityRequest {
    ClientId: string;
    ClientSecret: string;
    IdentityPublicKey: any;
    DevicePassword: string;
    IdentityVersion: number;
    SignedChallenge: any;
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
    token: TokenResponse;
    password: string;
}
