import { check } from "k6";
import { SharedArray } from "k6/data";
import { Options } from "k6/options";
import { AuthenticatedClient } from "../libs/backbone-client/authenticated-client";
import { DataRepresentationForEnmeshedPerformanceTestsLoads } from "../libs/data-loader/models";
import { loadDataRepresentation } from "../libs/file-utils";

export const options: Options = {
    scenarios: {
        constantRequestRate: {
            executor: "constant-arrival-rate",
            rate: 1,
            timeUnit: "5m",
            duration: "60m",
            preAllocatedVUs: 1
        }
    }
};

const snapshot = (__ENV.snapshot as string | undefined) ?? "light";

const pools = loadDataRepresentation(snapshot, [DataRepresentationForEnmeshedPerformanceTestsLoads.Identities]).ofTypes("a", "c").pools;

const testIdentities = new SharedArray("testIdentities", function () {
    return pools.flatMap((p) => p.identities);
});

let identityIterator = 0;

export default function (): void {
    const currentIdentity = testIdentities[identityIterator++];

    const username = currentIdentity.devices[0].username;
    const password = currentIdentity.devices[0].password;
    const client = new AuthenticatedClient(username, password);

    const createChallengeResult = client.getChallenge();

    check(createChallengeResult, {
        "challenge contains correct device": (r) => r.createdByDevice === currentIdentity.devices[0].deviceId,
        "challenge contains correct address": (r) => r.createdBy === currentIdentity.address,
        "challenge id is not empty": (r) => r.id !== ""
    });
}
