export interface Identity {
    devices: Device[];
    pool: Pool;
}

export interface Pool {
    name: string;
    identities: Identity[];
}

interface Device {
    username: string;
    password: string;
}
