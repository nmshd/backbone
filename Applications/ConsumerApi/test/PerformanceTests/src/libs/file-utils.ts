import { SharedArray } from "k6/data";
import papaparse from "papaparse";
import { DREPT, DREPTLoads, Identity, Pool } from "./drept";
import { CsvDatawalletModification as CSVDatawalletModification, CSVIdentity } from "./drept/csv-types";

/**
 *
 * @param folderName The name of the folder matching the name of the snapshot to load
 * @param whatoToLoad An array of {@link DREPTLoads} representing the entities to be loaded
 * @returns a DREPT populated according to @link{whatoToLoad}
 */
export function LoadDREPT(folderName: string, whatoToLoad: DREPTLoads[] = [DREPTLoads.Identities]): DREPT {
    const csvFilesPath = `../snapshots/${folderName}/csvs`;
    let pools: Pool[];

    const poolsReturn = new SharedArray("pools", function () {
        if (!whatoToLoad.includes(DREPTLoads.Identities)) {
            console.warn("whatToLoad does not include Identities but they must always be loaded. Loading either way...");
        }

        pools = LoadPoolsWithIdentities();

        if (whatoToLoad.includes(DREPTLoads.DatawalletModifications)) {
            console.log("Loading datawallet modifications");
            LoadDataWalletModifications();
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
}
