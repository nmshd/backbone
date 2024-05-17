import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { check } from "k6";
import exec from "k6/execution";
import { Response } from "k6/http";
import { ConstantArrivalRateScenario, Options } from "k6/options";
import { CreateIdentityResponse, IdentityWithToken } from "../models/identity";
import { StartSyncRunRequestBody, StartSyncRunResponse, SyncRunType } from "../models/sync-run";
import { createIdentity, exchangeToken } from "../libs/backbone-client/identity";

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

const client = new Httpx({
    baseURL: "http://localhost:8081/api/v1/",
    timeout: 20000 // 20s timeout
}) as any; // TODO: remove cast to any

export default function (testIdentities: IdentityWithToken[]): void {
    const dataWalletVersion = exec.vu.iterationInInstance + 3;
    const currentVuIdInTest = exec.vu.idInTest;
    const identity = testIdentities[currentVuIdInTest - 1];

    console.debug(`VU ${currentVuIdInTest} is using identity with address ${identity.response.address}`);

    const requestBody: StartSyncRunRequestBody = {
        duration: 10,
        type: SyncRunType.ExternalEventSync
    };

    const startSyncRunResponse = client.post("SyncRuns", JSON.stringify(requestBody), {
        headers: {
            /* eslint-disable @typescript-eslint/naming-convention */
            "Content-Type": "application/json",
            "X-Supported-Datawallet-Version": dataWalletVersion,
            Authorization: `Bearer ${identity.token.access_token}`
            /* eslint-enable @typescript-eslint/naming-convention */
        }
    }) as Response;

    check(startSyncRunResponse, {
        /* eslint-disable @typescript-eslint/naming-convention */
        "Start sync run": (r) => r.status === 200
        /* eslint-enable @typescript-eslint/naming-convention */
    });
}

export function setup(): IdentityWithToken[] {
    const scenario = exec.test.options.scenarios?.constantRequestRate as ConstantArrivalRateScenario;
    const testIdentities: IdentityWithToken[] = [];

    for (let i = 0; i < scenario.preAllocatedVUs; i++) {
        const { httpResponse, generatedPassword } = createIdentity(client, "test", "test");

        check(httpResponse, {
            /* eslint-disable @typescript-eslint/naming-convention */
            "Identity was created": (r) => r.status === 201
            /* eslint-enable @typescript-eslint/naming-convention */
        });

        const createdIdentityResponseValue = httpResponse.json("result") as unknown as CreateIdentityResponse | undefined;

        check(createdIdentityResponseValue, {
            /* eslint-disable @typescript-eslint/naming-convention */
            "response has Address": (r) => r?.address !== undefined,
            "response has device": (r) => r?.device !== undefined,
            "device has Id": (r) => r?.device.id !== undefined
            /* eslint-enable @typescript-eslint/naming-convention */
        });

        const token = exchangeToken(client, createdIdentityResponseValue!, generatedPassword);

        const requestBody: StartSyncRunRequestBody = {
            duration: 10,
            type: SyncRunType.DatawalletVersionUpgrade
        };

        const startSyncRunResponse = client.post("SyncRuns", JSON.stringify(requestBody), {
            headers: {
                /* eslint-disable @typescript-eslint/naming-convention */
                "Content-Type": "application/json",
                Authorization: `Bearer ${token.access_token}`
                /* eslint-enable @typescript-eslint/naming-convention */
            }
        }) as Response;

        check(startSyncRunResponse, {
            /* eslint-disable @typescript-eslint/naming-convention */
            "Start datawallet version upgrade": (r) => r.status === 201
            /* eslint-enable @typescript-eslint/naming-convention */
        });

        const startSyncRunResponseValue = startSyncRunResponse.json("result") as unknown as StartSyncRunResponse | undefined;

        const finalizeDatawalletVersionUpgradeResponse = client.put(
            `SyncRuns/${startSyncRunResponseValue?.syncRun?.id}/FinalizeDatawalletVersionUpgrade`,
            JSON.stringify({ newDatawalletVersion: 2, datawalletModifications: [] }),
            {
                headers: {
                    /* eslint-disable @typescript-eslint/naming-convention */
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token.access_token}`
                    /* eslint-enable @typescript-eslint/naming-convention */
                }
            }
        ) as Response;

        check(finalizeDatawalletVersionUpgradeResponse, {
            /* eslint-disable @typescript-eslint/naming-convention */
            "Finalize datawallet version upgrade": (r) => r.status === 200
            /* eslint-enable @typescript-eslint/naming-convention */
        });

        testIdentities.push({
            response: createdIdentityResponseValue!,
            token,
            password: generatedPassword
        });
    }
    console.log(`testIdentities has ${testIdentities.length} identities after setup completed`);
    return testIdentities;
}
