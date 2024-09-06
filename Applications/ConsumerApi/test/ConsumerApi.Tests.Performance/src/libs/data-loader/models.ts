export interface Identity {
    sentMessages?: Message[];
    address: string;
    devices: Device[];
    poolAlias: string;
    datawalletModifications?: DatawalletModification[];
    relationshipTemplates?: RelationshipTemplate[];
    relatedIdentities?: Relationship[];
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

interface Message {
    messageId: string;
    recipient: string;
}

interface Relationship {
    relationshipId: string;
    recipient: string;
}

interface DatawalletModification {
    modificationId: string;
    index: number;
}

interface RelationshipTemplate {
    relationshipTemplateId: string;
}

export interface IDataRepresentationForEnmeshedPerformanceTests {
    pools: Pool[];

    ofTypes(...types: string[]): IDataRepresentationForEnmeshedPerformanceTests;
}

export class DataRepresentationForEnmeshedPerformanceTests implements IDataRepresentationForEnmeshedPerformanceTests {
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
    public ofTypes = (...types: string[]): IDataRepresentationForEnmeshedPerformanceTests =>
        new DataRepresentationForEnmeshedPerformanceTests(this._pools.filter((pool) => types.some((type) => pool.name.startsWith(type))));
}

export enum DataRepresentationForEnmeshedPerformanceTestsLoads {
    Identities,
    Relationships,
    Messages,
    RelationshipTemplates,
    DatawalletModifications
}
