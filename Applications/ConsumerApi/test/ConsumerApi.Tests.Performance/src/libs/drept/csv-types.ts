// CSV types for parsing

export interface CSVIdentity {
    Address: string;
    DeviceId: string;
    Username: string;
    Password: string;
    Alias: string;
}

export interface CsvDatawalletModification {
    IdentityAddress: string;
    ModificationIndex: string;
    ModificationId: string;
}

export interface CSVRelationshipTemplate {
    IdentityAddress: string;
    RelationshipTemplateId: string;
}

export interface CSVRelationship {
    RelationshipId: string;
    AddressFrom: string;
    AddressTo: string;
}
export interface CSVMessage {
    MessageId: string;
    AddressFrom: string;
    AddressTo: string;
}
