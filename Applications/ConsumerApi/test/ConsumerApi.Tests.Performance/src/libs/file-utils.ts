import { SharedArray } from "k6/data";
import papaparse from "papaparse";
import { Identity, LoadedPools, Pool, PoolLoadOptions } from "./data-loader/models";

/**
 *
 * @param folderName The name of the folder matching the name of the snapshot to load
 * @param whatoToLoad An array of {@link PoolLoadOptions} representing the entities to be loaded
 * @returns a {@link LoadedPools} populated according to {@link whatoToLoad}
 */
export function loadPools(folderName: string, whatoToLoad: PoolLoadOptions[] = [PoolLoadOptions.Identities]): LoadedPools {
    const csvFilesPath = `../snapshots/${folderName}/csvs`;
    let pools: Pool[];
    const identitiesMap: Map<string, Identity> = new Map<string, Identity>();

    const poolsReturn = new SharedArray("pools", function () {
        if (!whatoToLoad.includes(PoolLoadOptions.Identities)) {
            console.warn("whatToLoad does not include Identities but they must always be loaded. Loading either way...");
        }

        console.info(`Started loading ${folderName} snapshot. Loading identities`);
        const start = +new Date();

        pools = loadPoolsWithIdentities();

        if (whatoToLoad.includes(PoolLoadOptions.DatawalletModifications)) {
            console.info("Loading datawallet modifications");
            loadDataWalletModifications();
        }

        if (whatoToLoad.includes(PoolLoadOptions.RelationshipTemplates)) {
            console.info("Loading relationship templates");
            loadRelationshipTemplates();
        }

        if (whatoToLoad.includes(PoolLoadOptions.Relationships)) {
            console.info("Loading relationships");
            loadRelationships();
        }

        if (whatoToLoad.includes(PoolLoadOptions.Messages)) {
            console.info("Loading messages");
            loadMessages();
        }

        console.info(`Finished Loading in ${+new Date() - start}ms`);
        return pools;
    });

    return new LoadedPools(poolsReturn);

    function loadDataWalletModifications() {
        const DatawalletModificationsFile = open(`${csvFilesPath}/datawalletModifications.csv`);
        const parsedDatawalletModifications = papaparse.parse<CSVDatawalletModification>(DatawalletModificationsFile, { header: true }).data.filter((x) => x.IdentityAddress !== "");

        // create a map of datawalletModifications by their address
        const datawalletModificationsMap = new Map<string, CSVDatawalletModification[]>();
        parsedDatawalletModifications.forEach((modification) => {
            if (!datawalletModificationsMap.has(modification.IdentityAddress)) {
                datawalletModificationsMap.set(modification.IdentityAddress, []);
            }
            datawalletModificationsMap.get(modification.IdentityAddress)!.push(modification);
        });

        // add datawalletModifications to each identity
        pools.forEach((pool) => {
            pool.identities.forEach((identity) => {
                identity.datawalletModifications = [];
                datawalletModificationsMap.get(identity.address)?.forEach((x) => {
                    identity.datawalletModifications!.push({ index: Number.parseInt(x.ModificationIndex), modificationId: x.ModificationId });
                });
            });
        });
    }

    function loadPoolsWithIdentities() {
        const identitiesFile = open(`${csvFilesPath}/identities.csv`);
        const parsedIdentities = papaparse.parse<CSVIdentity>(identitiesFile, { header: true }).data.filter((x) => x.Address !== "");

        parsedIdentities.forEach((csvIdentity) => {
            const device = { deviceId: csvIdentity.DeviceId, username: csvIdentity.Username, password: csvIdentity.Password };
            const sameIdentityInPool = identitiesMap.get(csvIdentity.Address);

            if (sameIdentityInPool) {
                sameIdentityInPool.devices.push(device);
            } else {
                identitiesMap.set(csvIdentity.Address, {
                    address: csvIdentity.Address,
                    devices: [device],
                    poolAlias: csvIdentity.Alias
                });
            }
        });

        const result: Pool[] = [];

        parsedIdentities.forEach((csvIdentity) => {
            let pool = result.find((p) => p.name === csvIdentity.Alias);
            if (!pool) {
                result.push({ name: csvIdentity.Alias, identities: [] });
                pool = result.find((p) => p.name === csvIdentity.Alias);
            }
        });

        result.forEach((pool) => {
            identitiesMap.forEach((i) => {
                if (i.poolAlias === pool.name) pool.identities.push(i);
            });
        });

        return result;
    }

    function loadRelationshipTemplates() {
        const RelationshipTemplatesFile = open(`${csvFilesPath}/relationshipTemplates.csv`);
        const parsedRelationshipTemplates = papaparse.parse<CSVRelationshipTemplate>(RelationshipTemplatesFile, { header: true }).data.filter((x) => x.IdentityAddress !== "");

        parsedRelationshipTemplates.forEach((relationshipTemplate) => {
            const identity = identitiesMap.get(relationshipTemplate.IdentityAddress);
            if (identity !== undefined) {
                identity.relationshipTemplates ??= [];
                identity.relationshipTemplates.push({ relationshipTemplateId: relationshipTemplate.RelationshipTemplateId });
            }
        });
    }

    function loadRelationships() {
        const RelationshipsFile = open(`${csvFilesPath}/relationships.csv`);
        const parsedRelationships = papaparse.parse<CSVRelationship>(RelationshipsFile, { header: true }).data.filter((x) => x.RelationshipId !== "");
        parsedRelationships.forEach((relationship) => {
            const identityFrom = identitiesMap.get(relationship.AddressFrom)!;
            identityFrom.relationships ??= [];
            identityFrom.relationships.push({ fromAddress: relationship.AddressFrom, toAddress: relationship.AddressTo, relationshipId: relationship.RelationshipId });

            const identityTo = identitiesMap.get(relationship.AddressTo)!;
            identityTo.relationships ??= [];
            identityTo.relationships.push({ fromAddress: relationship.AddressTo, toAddress: relationship.AddressFrom, relationshipId: relationship.RelationshipId });
        });
    }

    function loadMessages() {
        const MessagesFile = open(`${csvFilesPath}/messages.csv`);
        const parsedMessages = papaparse.parse<CSVMessage>(MessagesFile, { header: true }).data.filter((x) => x.AddressFrom !== "");
        parsedMessages.forEach((message) => {
            const identityFrom = identitiesMap.get(message.AddressFrom);
            const identityTo = identitiesMap.get(message.AddressTo);
            if (identityTo && identityFrom) {
                identityFrom.sentMessages ??= [];
                identityFrom.sentMessages.push({ messageId: message.AddressFrom, recipient: message.AddressTo });
            }
        });
    }
}

interface CSVIdentity {
    Address: string;
    DeviceId: string;
    Username: string;
    Password: string;
    Alias: string;
}

interface CSVDatawalletModification {
    IdentityAddress: string;
    ModificationIndex: string;
    ModificationId: string;
}

interface CSVRelationshipTemplate {
    IdentityAddress: string;
    RelationshipTemplateId: string;
}

interface CSVRelationship {
    RelationshipId: string;
    AddressFrom: string;
    AddressTo: string;
}
interface CSVMessage {
    MessageId: string;
    AddressFrom: string;
    AddressTo: string;
}
