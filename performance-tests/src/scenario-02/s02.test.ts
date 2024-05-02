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
            preAllocatedVUs: 20,
            maxVUs: 100
        }
    }
};

const session = new Httpx({
    baseURL: "http://localhost:8081/api/v1/",
    timeout: 20000 // 20s timeout.
});

const testIdentities: Response[] = [];

export default async function () {
    const identity = testIdentities.shift();

    while (true) {
        const requestBody: StartSyncRunRequestBody = {
            duration: 10,
            type: SyncRunType.ExternalEventSync
        };

        // session.post("Identities", JSON.stringify(requestBody), {
        //     headers: {
        //         "Content-Type": "application/json"
        //     }
        // }) as Response;

        sleep(1000);
    }
}

export function setup() {
    const mainScenario = exec.test.options.scenarios?.constant_request_rate as ConstantArrivalRateScenario;

    for (let i = 0; i < (mainScenario.maxVUs ?? 100); i++) {
        const createdIdentityResponse = CreateIdentity("test", "test");

        check(createdIdentityResponse, {
            "Identity was created": (r) => r.status === 201
        });

        const createdIdentityResponseValue = createdIdentityResponse.json("result") as unknown as CreateIdentityResponse;

        check(createdIdentityResponseValue, {
            "response has Address": (r) => r.address != undefined,
            "response has device": (r) => r.device != undefined,
            "device has Id": (r) => r.device.id != undefined
        });

        const token = ExchangeToken(createdIdentityResponse);

        testIdentities.push(createdIdentityResponse);
    }
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

enum SyncRunType {
    ExternalEventSync,
    DatawalletVersionUpgrade
}

function CreateIdentity(ClientId: string, ClientSecret: string): Response {
    const sidecar = new Sidecar();

    const challenge = getChallenge();

    const keyPair = sidecar.GenerateKeyPair();

    const signedChallenge = sidecar.SignChallenge(keyPair, challenge);

    const password = sidecar.GeneratePassword();

    const createIdentityRequest: CreateIdentityRequest = {
        ClientId,
        ClientSecret,
        SignedChallenge: { challenge: JSON.stringify(challenge), signature: b64encode(JSON.stringify(signedChallenge)) },
        IdentityPublicKey: b64encode(JSON.stringify(keyPair.pub)),
        DevicePassword: password,
        IdentityVersion: 1
    };

    const createdIdentityResponse = session.post("Identities", JSON.stringify(createIdentityRequest), { headers: { "Content-Type": "application/json" } }) as Response;
    return createdIdentityResponse;
}

function getChallenge(): ChallengeRequestRepresentation {
    const receivedChallenge = session.post("Challenges").json("result") as ChallengeResponse;

    return {
        expiresAt: receivedChallenge.expiresAt,
        id: receivedChallenge.id,
        type: "Identity"
    };
}
function ExchangeToken(createdIdentityResponse: Response) {
    const payload = {
        client_id: "test",
        client_secret: "test",
        grant_type: "password",
        username: createdIdentityResponse.json()
    };
    const token = session.post("connect/token", JSON.stringify(payload), { headers: { "Content-Type": "application/json" } });
    console.log(token);
}
