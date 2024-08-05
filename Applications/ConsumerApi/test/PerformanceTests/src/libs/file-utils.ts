import { SharedArray } from "k6/data";
import papaparse from "papaparse";
import { DREPT, DREPTLoads, Identity, Pool } from "./drept";
import { CsvDatawalletModification as CSVDatawalletModification, CSVIdentity, CSVMessage, CSVRelationship, CSVRelationshipTemplate } from "./drept/csv-types";

/**
 *
 * @param folderName The name of the folder matching the name of the snapshot to load
 * @param whatoToLoad An array of {@link DREPTLoads} representing the entities to be loaded
 * @returns a DREPT populated according to @link{whatoToLoad}
 */
export function LoadDREPT(folderName: string, whatoToLoad: DREPTLoads[] = [DREPTLoads.Identities]): DREPT {
    const csvFilesPath = `../snapshots/${folderName}/csvs`;
    let pools: Pool[];
    let identitiesMap: Map<string, Identity>;

    const poolsReturn = new SharedArray("pools", function () {
        if (!whatoToLoad.includes(DREPTLoads.Identities)) {
            console.warn("whatToLoad does not include Identities but they must always be loaded. Loading either way...");
        }

        pools = LoadPoolsWithIdentities();
        identitiesMap = PopulateIdentitiesMap();

        if (whatoToLoad.includes(DREPTLoads.DatawalletModifications)) {
            console.log("Loading datawallet modifications");
            LoadDataWalletModifications();
        }

        if (whatoToLoad.includes(DREPTLoads.RelationshipTemplates)) {
            console.log("Loading relationship templates");
            LoadRelationshipTemplates();
        }

        if (whatoToLoad.includes(DREPTLoads.Relationships)) {
            console.log("Loading relationships");
            LoadRelationships();
        }

        if (whatoToLoad.includes(DREPTLoads.Messages)) {
            console.log("Loading messages");
            LoadMessages();
        }

        return pools;
    });

    return new DREPT(poolsReturn);

    function LoadDataWalletModifications() {
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

    function LoadPoolsWithIdentities() {
        const identitiesFile = open(`${csvFilesPath}/identities.csv`);
        const parsedIdentities = papaparse.parse<CSVIdentity>(identitiesFile, { header: true }).data.filter((x) => x.Address !== "");

        const identities: Identity[] = [];
        parsedIdentities.forEach((csvIdentity) => {
            const identity: Identity = {
                address: csvIdentity.Address,
                devices: [{ deviceId: csvIdentity.DeviceId, username: csvIdentity.Username, password: csvIdentity.Password }],
                poolAlias: csvIdentity.Alias
            };
            identities.push(identity);
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
            pool.identities = identities.filter((i) => i.poolAlias === pool.name);
        });

        return result;
    }

    function LoadRelationshipTemplates() {
        const RelationshipTemplatesFile = open(`${csvFilesPath}/relationshipTemplates.csv`);
        const parsedRelationshipTemplates = papaparse.parse<CSVRelationshipTemplate>(RelationshipTemplatesFile, { header: true }).data.filter((x) => x.IdentityAddress !== "");

        pools.forEach((pool) => {
            pool.identities.forEach((identity) => {
                identity.relationshipTemplates = [];
                parsedRelationshipTemplates.forEach((relationshipTemplate) => {
                    identity.relationshipTemplates!.push({ relationshipTemplateId: relationshipTemplate.RelationshipTemplateId });
                });
            });
        });
    }

    function LoadRelationships() {
        const RelationshipsFile = open(`${csvFilesPath}/relationships.csv`);
        const parsedRelationships = papaparse.parse<CSVRelationship>(RelationshipsFile, { header: true }).data.filter((x) => x.AddressFrom !== "");
        parsedRelationships.forEach((relationship) => {
            const identityFrom = identitiesMap.get(relationship.AddressFrom);
            const identityTo = identitiesMap.get(relationship.AddressTo);
            if (identityTo && identityFrom) {
                identityFrom.relatedIdentities ??= [];
                identityTo.relatedIdentities ??= [];
                identityFrom.relatedIdentities.push({ recipient: identityTo.address, relationshipId: relationship.RelationshipId });
                identityTo.relatedIdentities.push({ recipient: identityFrom.address, relationshipId: relationship.RelationshipId });
            }
        });
    }

    function LoadMessages() {
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

    function PopulateIdentitiesMap(): Map<string, Identity> {
        const result = new Map<string, Identity>();
        pools.forEach((pool) => {
            pool.identities.forEach((identity) => {
                result.set(identity.address, identity);
            });
        });
        return result;
    }
}
