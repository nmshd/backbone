import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { check } from "k6";
import { b64encode } from "k6/encoding";
import exec from "k6/execution";
import { Response } from "k6/http";
import { ConstantArrivalRateScenario, Options } from "k6/options";
import { ChallengeResponse } from "../domain/challenge";
import { CreateIdentityRequest, CreateIdentityResponse, IdentityWithToken } from "../domain/identity";
import { StartSyncRunRequestBody, StartSyncRunResponse, SyncRunType } from "../domain/sync-runs";
import { ChallengeRequestRepresentation, CryptoHelper } from "../libs/crypto-helper";

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
});

export default async function (testIdentities: IdentityWithToken[]) {
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
        const { httpResponse, generatedPassword } = CreateIdentity("test", "test");

        check(httpResponse, {
            "Identity was created": (r) => r.status === 201
        });

        const createdIdentityResponseValue = httpResponse.json("result") as unknown as CreateIdentityResponse;

        check(createdIdentityResponseValue, {
            "response has Address": (r) => r.address != undefined,
            "response has device": (r) => r.device != undefined,
            "device has Id": (r) => r.device.id != undefined
        });

        const token = ExchangeToken(createdIdentityResponseValue, generatedPassword);

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

        console.log(startSyncRunResponse.status);

        check(startSyncRunResponse, {
            "Start datawallet version upgrade": (r) => r.status === 200
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

function CreateIdentity(ClientId: string, ClientSecret: string): { httpResponse: Response; generatedPassword: string } {
    const sidecar = new CryptoHelper();

    try {
        const challenge = getChallenge();

        const keyPair = sidecar.GenerateKeyPair();

        const signedChallenge = sidecar.SignChallenge(keyPair, challenge);

        const generatedPassword = sidecar.GeneratePassword();

        const createIdentityRequest: CreateIdentityRequest = {
            ClientId,
            ClientSecret,
            SignedChallenge: { challenge: JSON.stringify(challenge), signature: b64encode(JSON.stringify(signedChallenge)) },
            IdentityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
            DevicePassword: generatedPassword,
            IdentityVersion: 1
        };

        const httpResponse = client.post("Identities", JSON.stringify(createIdentityRequest), { headers: { "Content-Type": "application/json" } }) as Response;
        return { httpResponse, generatedPassword };
    } catch (e) {
        console.error(e);
        throw e;
    }
}

function getChallenge(): ChallengeRequestRepresentation {
    const receivedChallenge = client.post("Challenges").json("result") as ChallengeResponse;

    return {
        expiresAt: receivedChallenge.expiresAt,
        id: receivedChallenge.id,
        type: "Identity"
    };
}
function ExchangeToken(createdIdentityResponse: CreateIdentityResponse, password: string) {
    const payload = {
        client_id: "test",
        client_secret: "test",
        grant_type: "password",
        username: createdIdentityResponse.device.username,
        password
    };
    return client.post("http://localhost:8081/connect/token", payload, { headers: { "Content-Type": "application/x-www-form-urlencoded" } }).json() as TokenResponse;
}
