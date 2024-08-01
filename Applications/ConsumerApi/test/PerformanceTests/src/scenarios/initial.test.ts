import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { default as exec } from "k6/execution";
import { ConstantArrivalRateScenario, Options } from "k6/options";
import { exchangeToken } from "../libs/backbone-client";
import { LoadDREPT } from "../libs/file-utils";
import { IdentityWithToken } from "../models";

export const options: Options = {
    scenarios: {
        constantRequestRate: {
            executor: "constant-arrival-rate",
            rate: 10,
            timeUnit: "1s",
            duration: "1m",
            preAllocatedVUs: 10
        }
    }
};

const pools = LoadDREPT().pools.filter((pool) => pool.name.startsWith("a") || pool.name.startsWith("c"));

const client = new Httpx({
    baseURL: "http://localhost:8081/",
    timeout: 20000 // 20s timeout
});

export default function (testIdentities: IdentityWithToken[]): void {
    const currentVuIdInTest = exec.vu.idInTest;
    const identity = testIdentities[currentVuIdInTest - 1];

    console.debug(`VU ${currentVuIdInTest} is using identity with address ${identity.response.address}`);
}

export function setup(): IdentityWithToken[] {
    const scenario = exec.test.options.scenarios?.constantRequestRate as ConstantArrivalRateScenario;
    const testIdentities = pools.flatMap((p) => p.identities) as IdentityWithToken[];

    for (let i = 0; i < scenario.preAllocatedVUs; i++) {
        const username = testIdentities[i].devices[0].username;
        const password = testIdentities[i].devices[0].password;
        const token = exchangeToken(client, username, password);

        testIdentities[i].token = token;
    }
    console.log(`testIdentities has ${testIdentities.length} identities after setup completed`);
    return testIdentities;
}
