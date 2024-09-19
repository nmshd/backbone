import { check } from "k6";
import { SharedArray } from "k6/data";
import { Options } from "k6/options";
import { AuthenticatedClient } from "../libs/backbone-client/authenticated-client";
import { HttpClientConfiguration } from "../libs/backbone-client/http-client-configuration";
import { PoolLoadOptions, PoolTypes } from "../libs/data-loader/models";
import { loadPools } from "../libs/file-utils";

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

const pools = loadPools(snapshot, [PoolLoadOptions.Identities]).ofTypes(PoolTypes.App, PoolTypes.Connector).pools;

const testIdentities = new SharedArray("testIdentities", function () {
    return pools.flatMap((p) => p.identities);
});

let identityIterator = 0;

export default function (): void {
    const thisIdentityIterator = identityIterator++;
    const currentIdentity = testIdentities[thisIdentityIterator];

    const config = new HttpClientConfiguration();
    config.timeout = 1000;

    const username = currentIdentity.devices[0].username;
    const password = currentIdentity.devices[0].password;
    const client = new AuthenticatedClient(username, password, config);

    const createChallengeResult = client.getChallenge();

    check(createChallengeResult, {
        "challenge contains correct device": (r) => r.createdByDevice === currentIdentity.devices[0].deviceId,
        "challenge contains correct address": (r) => r.createdBy === currentIdentity.address,
        "challenge id is not empty": (r) => r.id !== ""
    });
}
