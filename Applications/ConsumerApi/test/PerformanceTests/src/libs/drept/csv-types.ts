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
