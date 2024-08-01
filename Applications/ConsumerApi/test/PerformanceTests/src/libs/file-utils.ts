import { SharedArray } from "k6/data";
import papaparse from "papaparse";
import { DREPT, Identity, Pool } from "./drept";
import { CSVIdentity } from "./drept/csv-types";

export function LoadDREPT(folderName: string): DREPT {
    const csvFilesPath = `../snapshots/${folderName}/csvs`;

    const identities = new SharedArray("identities", function () {
        const identitiesFile = open(`${csvFilesPath}/identities.csv`);
        const parsedIdentities = papaparse.parse<CSVIdentity>(identitiesFile, { header: true }).data.filter((identity) => identity.Address !== "");
        console.log(`found ${parsedIdentities.length} identities`);
        const result: Identity[] = [];
        parsedIdentities.forEach((csvIdentity) => {
            const identity: Identity = {
                address: csvIdentity.Address,
                devices: [{ deviceId: csvIdentity.DeviceId, username: csvIdentity.Username, password: csvIdentity.Password }],
                poolAlias: csvIdentity.Alias
            };
            result.push(identity);
        });

        return result;
    });

    const pools = new SharedArray("pools", function () {
        const identitiesFile = open(`${csvFilesPath}/identities.csv`);
        const parsedIdentities = papaparse.parse<CSVIdentity>(identitiesFile, { header: true }).data.filter((identity) => identity.Address !== "");
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
    });

    return new DREPT(pools);
}
