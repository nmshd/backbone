// DATA REPRESENTATION FOR ENMESHED PERFORMANCE TESTS PROTOCOL

export interface Identity {
    address: string;
    devices: Device[];
    poolAlias: string;
    datawalletModifications?: DatawalletModification[];
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

interface DatawalletModification {
    modificationId: string;
    index: number;
}

export interface IDREPT {
    pools: Pool[];

    ofTypes(...types: string[]): IDREPT;
}

export class DREPT implements IDREPT {
    private readonly _pools: Pool[] = [];

    public constructor(pools: Pool[]) {
        this._pools = pools;
    }

    public get pools(): Pool[] {
        return this._pools;
    }

    /**
     * fluent method to filter the loaded pools
     * @param types the names of the pools to be loaded, or simply theIR initial letter(s), e.g.: `.ofTypes("a", "c")` -- loads pools of type a1, a2, a.., c1, c2, c..
     * @returns
     */
    public ofTypes = (...types: string[]): IDREPT => new DREPT(this._pools.filter((pool) => types.some((type) => pool.name.startsWith(type))));
}

export enum DREPTLoads {
    Identities,
    Relationships,
    Messages,
    RelationshipTemplates,
    DatawalletModifications
}
