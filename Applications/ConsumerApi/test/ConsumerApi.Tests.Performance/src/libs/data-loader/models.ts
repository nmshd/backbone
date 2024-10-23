export interface Identity {
    relationships?: Relationship[];
    sentMessages?: Message[];
    address: string;
    devices: Device[];
    poolAlias: string;
    datawalletModifications?: DatawalletModification[];
    relationshipTemplates?: RelationshipTemplate[];
}

export interface Pool {
    name: string;
    identities: Identity[];
}

export interface Relationship {
    relationshipId: string;
    fromAddress: string;
    toAddress: string;
}

interface Device {
    deviceId: string;
    username: string;
    password: string;
}

interface Message {
    messageId: string;
    recipient: string;
}

interface DatawalletModification {
    modificationId: string;
    index: number;
}

interface RelationshipTemplate {
    relationshipTemplateId: string;
}

export class LoadedPools {
    private readonly _pools: Pool[] = [];

    public constructor(pools: Pool[]) {
        this._pools = pools;
    }

    public get pools(): Pool[] {
        return this._pools;
    }

    /**
     * fluent method to filter the loaded pools
     * @param poolTypes the types of pools to be loaded
     * @returns
     */
    public ofTypes(...poolTypes: PoolTypes[]): LoadedPools {
        if (poolTypes.includes(PoolTypes.All)) return new LoadedPools(this._pools);

        return new LoadedPools(this._pools.filter((pool) => poolTypes.some((type) => pool.name.startsWith(type))));
    }
}

export enum PoolLoadOptions {
    Identities,
    Relationships,
    Messages,
    RelationshipTemplates,
    DatawalletModifications
}

export enum PoolTypes {
    App = "a",
    AppLight = "a1",
    AppMedium = "a2",
    AppHeavy = "a3",

    Connector = "c",
    ConnectorLight = "c1",
    ConnectorMedium = "c2",
    ConnectorHeavy = "c3",

    Never = "e",

    All = "*"
}
