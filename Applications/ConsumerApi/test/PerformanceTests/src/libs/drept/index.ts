// DATA REPRESENTATION FOR ENMESHED PERFORMANCE TESTS PROTOCOL

export interface Identity {
    address: string;
    devices: Device[];
    poolAlias: string;
}

export interface Pool {
    name: string;
    identities: Identity[];
}

interface Device {
    deviceId: string;
    username: string;
    password: string;
}

export interface DREPT {
    pools: Pool[];
}
