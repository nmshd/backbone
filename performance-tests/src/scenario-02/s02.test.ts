import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { check } from "k6";
import exec from "k6/execution";
import { Response } from "k6/http";
import { ConstantArrivalRateScenario, Options } from "k6/options";
import { CreateIdentityResponse, IdentityWithToken } from "../domain/identity";
import { StartSyncRunRequestBody, StartSyncRunResponse, SyncRunType } from "../domain/sync-runs";
import { CreateIdentity, ExchangeToken } from "../libs/backbone-client/identity";
import { HttpxClient } from "../libs/k6-utils";

export const options: Options = {
    scenarios: {
        constant_request_rate: {
            executor: "constant-arrival-rate",
            rate: 10,
            timeUnit: "1s",
            duration: "1m",
            preAllocatedVUs: 10
        }
    }
};

const client = new Httpx({
    baseURL: "http://localhost:8081/api/v1/",
    timeout: 20000 // 20s timeout.
}) as HttpxClient;

export default async function (testIdentities: IdentityWithToken[]) {
    const dataWalletVersion = exec.vu.iterationInInstance + 3;
    const currentVuIdInTest = exec.vu.idInTest;
    const identity = testIdentities[currentVuIdInTest - 1];

    console.debug(`VU ${currentVuIdInTest} is using identity with address ${identity?.response.address}`);

    if (identity == undefined) {
        return;
    }

    const requestBody: StartSyncRunRequestBody = {
        duration: 10,
        type: SyncRunType.ExternalEventSync
    };

    const startSyncRunResponse = client.post("SyncRuns", JSON.stringify(requestBody), {
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
    const scenario = exec.test.options.scenarios?.constant_request_rate as ConstantArrivalRateScenario;
    const testIdentities = [];

    for (let i = 0; i < scenario.preAllocatedVUs; i++) {
        const { httpResponse, generatedPassword } = CreateIdentity(client, "test", "test");

        check(httpResponse, {
            "Identity was created": (r) => r.status === 201
        });

        const createdIdentityResponseValue = httpResponse.json("result") as unknown as CreateIdentityResponse;

        check(createdIdentityResponseValue, {
            "response has Address": (r) => r.address != undefined,
            "response has device": (r) => r.device != undefined,
            "device has Id": (r) => r.device.id != undefined
        });

        const token = ExchangeToken(client, createdIdentityResponseValue, generatedPassword);

        const requestBody: StartSyncRunRequestBody = {
            duration: 10,
            type: SyncRunType.DatawalletVersionUpgrade
        };

        const startSyncRunResponse = client.post("SyncRuns", JSON.stringify(requestBody), {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token.access_token}`
            }
        }) as Response;

        check(startSyncRunResponse, {
            "Start datawallet version upgrade": (r) => r.status === 201
        });

        const startSyncRunResponseValue = startSyncRunResponse.json("result") as unknown as StartSyncRunResponse;

        const finalizeDatawalletVersionUpgradeResponse = client.put(
            `SyncRuns/${startSyncRunResponseValue.syncRun?.id}/FinalizeDatawalletVersionUpgrade`,
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
            response: createdIdentityResponseValue,
            token,
            password: generatedPassword
        });
    }
    console.log(`testIdentities has ${testIdentities.length} identities after setup completed`);
    return testIdentities;
}
