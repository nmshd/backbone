import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { check, sleep } from "k6";
import { b64encode } from "k6/encoding";
import exec from "k6/execution";
import { Response } from "k6/http";
import { ConstantArrivalRateScenario, Options } from "k6/options";
import { ChallengeRequestRepresentation, Sidecar } from "../libs/sidecar";

export const options: Options = {
    scenarios: {
        constant_request_rate: {
            executor: "constant-arrival-rate",
            rate: 8,
            timeUnit: "1s",
            duration: "1m",
            preAllocatedVUs: 2,
            maxVUs: 5
        }
    }
};

const session = new Httpx({
    baseURL: "http://localhost:8081/api/v1/",
    timeout: 20000 // 20s timeout.
});

export default async function (testIdentities: IdentityWithToken[]) {
    const currentVuIdInTest = exec.vu.idInTest;
    const identity = testIdentities[currentVuIdInTest - 1];
    const dataWalletVersion = exec.vu.iterationInInstance;

    console.debug(`VU ${currentVuIdInTest} is using identity with address ${identity?.response.address}`);

    if (identity == undefined) {
        return;
    }

    const requestBody: StartSyncRunRequestBody = {
        duration: 10,
        type: SyncRunType.DatawalletVersionUpgrade
    };

    const startSyncRunResponse = session.post("SyncRuns", JSON.stringify(requestBody), {
        headers: {
            "Content-Type": "application/json",
            "X-Supported-Datawallet-Version": dataWalletVersion,
            Authorization: `Bearer ${identity.token.access_token}`
        }
    }) as Response;

    const startSyncRunResponseValue = startSyncRunResponse.json("result") as unknown as StartSyncRunResponse;

    check(startSyncRunResponse, {
        "SyncRun was started": (r) => r.status === 201
    });

    check(startSyncRunResponseValue, {
        "response has id": (r) => r.syncRun?.id != undefined
    });

    const finalizeDatawalletVersionUpgradeResponse = session.put(
        `SyncRuns/${startSyncRunResponseValue.syncRun?.id}/FinalizeDatawalletVersionUpgrade`,
        JSON.stringify({ newDatawalletVersion: dataWalletVersion + 1, datawalletModifications: [] } as FinalizeDatawalletVersionUpgradeRequest),
        {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${identity.token.access_token}`
            }
        }
    ) as Response;

    const finalizeDatawalletVersionUpgradeResponseValue = finalizeDatawalletVersionUpgradeResponse.json("result") as unknown as FinalizeDatawalletVersionUpgradeResponse;

    check(finalizeDatawalletVersionUpgradeResponse, {
        "SyncRun was finalized": (r) => r.status === 200
    });

    check(finalizeDatawalletVersionUpgradeResponseValue, {
        "response has newDatawalletVersion": (r) => r.newDatawalletVersion != undefined
    });

    sleep(1);
}

export function setup(): IdentityWithToken[] {
    const mainScenario = exec.test.options.scenarios?.constant_request_rate as ConstantArrivalRateScenario;
    const testIdentities = [];

    for (let i = 0; i < (mainScenario.maxVUs ?? 100); i++) {
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

        testIdentities.push({
            response: createdIdentityResponseValue,
            token,
            password: generatedPassword
        });
    }
    console.log(`testIdentities has ${testIdentities.length} identities after setup completed`);
    return testIdentities;
}

interface ChallengeResponse {
    id: string;
    expiresAt: string;
    createdBy: string;
    createdByDevice: string;
}

interface CreateIdentityRequest {
    ClientId: string;
    ClientSecret: string;
    IdentityPublicKey: any;
    DevicePassword: string;
    IdentityVersion: number;
    SignedChallenge: any;
}

interface StartSyncRunRequestBody {
    type: SyncRunType;
    duration: number;
}

interface CreateIdentityResponse {
    address: string;
    createdAt: string;
    device: {
        createdAt: string;
        id: string;
        username: string;
    };
}

interface TokenResponse {
    access_token: string;
    token_type: string;
    expires_in: number;
}

enum SyncRunType {
    ExternalEventSync,
    DatawalletVersionUpgrade
}

interface IdentityWithToken {
    response: CreateIdentityResponse;
    token: TokenResponse;
    password: string;
}

interface FinalizeDatawalletVersionUpgradeResponse {
    newDatawalletVersion: number;
    datawalletModifications: {
        id: string;
        index: number;
        createdAt: string;
    }[];
}

interface FinalizeDatawalletVersionUpgradeRequest {
    newDatawalletVersion: number;
    datawalletModifications?: CreateDatawalletModificationsRequestItem[];
}

interface CreateDatawalletModificationsRequestItem {
    objectIdentifier: string;
    payloadCategory?: string;
    collection: string;
    type: string;
    encryptedPayload?: string;
    datawalletVersion: number;
}

interface BackboneSyncRun {
    id: string;
    expiresAt: string;
    index: number;
    createdAt: string;
    createdBy: string;
    createdByDevice: string;
    eventCount: number;
}

interface StartSyncRunResponse {
    status: StartSyncRunStatus;
    syncRun: BackboneSyncRun | null;
}
interface StartSyncRunRequest {
    type: SyncRunType;
    duration?: number;
}

declare enum StartSyncRunStatus {
    Created = "Created",
    NoNewEvents = "NoNewEvents"
}

function CreateIdentity(ClientId: string, ClientSecret: string): { httpResponse: Response; generatedPassword: string } {
    const sidecar = new Sidecar();

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

    const httpResponse = session.post("Identities", JSON.stringify(createIdentityRequest), { headers: { "Content-Type": "application/json" } }) as Response;
    return { httpResponse, generatedPassword };
}

function getChallenge(): ChallengeRequestRepresentation {
    const receivedChallenge = session.post("Challenges").json("result") as ChallengeResponse;

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
    return session.post("http://localhost:8081/connect/token", payload, { headers: { "Content-Type": "application/x-www-form-urlencoded" } }).json() as TokenResponse;
}
