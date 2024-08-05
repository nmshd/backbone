import { Httpx } from "https://jslib.k6.io/httpx/0.1.0/index.js";
import { check } from "k6";
import { SharedArray } from "k6/data";
import { Options } from "k6/options";
import { apiVersion, exchangeToken } from "../libs/backbone-client";
import { DREPTLoads } from "../libs/drept";
import { LoadDREPT } from "../libs/file-utils";
import { CreateChallengeResponse } from "../models";

export const options: Options = {
    scenarios: {
        constantRequestRate: {
            executor: "constant-arrival-rate",
            rate: 1,
            timeUnit: "5m",
            duration: "60m",
            preAllocatedVUs: 20
        }
    }
};

const snapshot = (__ENV.snapshot as string | undefined) ?? "light";
const pools = LoadDREPT(snapshot, [DREPTLoads.Identities, DREPTLoads.DatawalletModifications]).ofTypes("a", "c").pools;

const testIdentities = new SharedArray("testIdentities", function () {
    return pools.flatMap((p) => p.identities);
});

const client = new Httpx({
    baseURL: "http://localhost:8081/",
    timeout: 20000 // 20s timeout
});

let identityIterator = 0;

export default function (): void {
    const currentIdentity = testIdentities[identityIterator++];

    const username = currentIdentity.devices[0].username;
    const password = currentIdentity.devices[0].password;
    const token = exchangeToken(client, username, password);

    const createChallengeResponse = client.post<"basic">(`api/${apiVersion}/Challenges`, null, {
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token.access_token}`
        }
    });

    check(createChallengeResponse, {
        "response code was 201": (r) => r.status === 201
    });

    const createChallengeResult = createChallengeResponse.json("result") as unknown as CreateChallengeResponse;

    check(createChallengeResult, {
        "challenge contains correct device": (r) => r.createdByDevice === currentIdentity.devices[0].deviceId,
        "challenge contains correct address": (r) => r.createdBy === currentIdentity.address,
        "challenge id is not empty": (r) => r.id !== ""
    });
}
