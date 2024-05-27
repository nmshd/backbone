import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { check } from "k6";
import exec from "k6/execution";
import { Response } from "k6/http";
import { ConstantArrivalRateScenario, Options } from "k6/options";
import { createIdentity, exchangeToken } from "../libs/backbone-client/identity";
import { CreateIdentityResponse, IdentityWithToken } from "../models/identity";
import { StartSyncRunRequestBody, StartSyncRunResponse, SyncRunType } from "../models/sync-run";

const ClientId = "test";
const ClientSecret = "test";

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

const apiVersion = "v1";

const client = new Httpx({
    baseURL: "http://localhost:8081/",
    timeout: 20000 // 20s timeout
}) as Httpx;

export default function (testIdentities: IdentityWithToken[]): void {
    const dataWalletVersion = exec.vu.iterationInInstance + 3;
    const currentVuIdInTest = exec.vu.idInTest;
    const identity = testIdentities[currentVuIdInTest - 1];

    console.debug(`VU ${currentVuIdInTest} is using identity with address ${identity.response.address}`);

    const requestBody: StartSyncRunRequestBody = {
        duration: 10,
        type: SyncRunType.ExternalEventSync
    };

    const startSyncRunResponse = client.post(`api/${apiVersion}/SyncRuns`, JSON.stringify(requestBody), {
        headers: {
            "Content-Type": "application/json",
            "X-Supported-Datawallet-Version": dataWalletVersion,
            Authorization: `Bearer ${identity.token.access_token}`
        }
    }) as Response;

    check(startSyncRunResponse, {
        "Start sync run": (r) => r.status === 200
    });
}

export function setup(): IdentityWithToken[] {
    const scenario = exec.test.options.scenarios?.constantRequestRate as ConstantArrivalRateScenario;
    const testIdentities: IdentityWithToken[] = [];

    for (let i = 0; i < scenario.preAllocatedVUs; i++) {
        const password = "test-password";
        const httpResponse = createIdentity(client, ClientId, ClientSecret, password);

        check(httpResponse, {
            "Identity was created": (r) => r.status === 201
        });

        const createdIdentityResponseValue = httpResponse.json("result") as unknown as CreateIdentityResponse | undefined;

        check(createdIdentityResponseValue, {
            "Response has Address": (r) => r?.address !== undefined,
            "Response has device": (r) => r?.device !== undefined,
            "Device has Id": (r) => r?.device.id !== undefined
        });

        const token = exchangeToken(client, createdIdentityResponseValue!.device.username, password);

        const requestBody: StartSyncRunRequestBody = {
            duration: 10,
            type: SyncRunType.DatawalletVersionUpgrade
        };

        const startSyncRunResponse = client.post(`api/${apiVersion}/SyncRuns`, JSON.stringify(requestBody), {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token.access_token}`
            }
        }) as Response;

        check(startSyncRunResponse, {
            "Start datawallet version upgrade": (r) => r.status === 201
        });

        const startSyncRunResponseValue = startSyncRunResponse.json("result") as unknown as StartSyncRunResponse | undefined;

        const finalizeDatawalletVersionUpgradeResponse = client.put(
            `api/${apiVersion}/SyncRuns/${startSyncRunResponseValue?.syncRun?.id}/FinalizeDatawalletVersionUpgrade`,
            JSON.stringify({ newDatawalletVersion: 2, datawalletModifications: [] }),
            {
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token.access_token}`
                }
            }
        ) as Response;

        check(finalizeDatawalletVersionUpgradeResponse, {
            "Finalize datawallet version upgrade": (r) => r.status === 200
        });

        testIdentities.push({
            response: createdIdentityResponseValue!,
            token,
            password
        });
    }
    console.log(`testIdentities has ${testIdentities.length} identities after setup completed`);
    return testIdentities;
}
