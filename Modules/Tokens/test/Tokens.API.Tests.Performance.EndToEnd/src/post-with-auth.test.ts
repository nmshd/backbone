import http from "k6/http";
import { Options } from "k6/options";
import { describe, expect } from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import { getAuthorizationHeader, getConfiguration, Size, tomorrow } from "./utils";

const configuration = getConfiguration();

const apiEndpoint = configuration.Host + "/api/v1";

interface Data {
    authToken: string;
}

function size(): Size {
    switch (configuration.Size) {
        case "S":
            return { vus: 1, iterations: 1 };
        case "M":
            return { vus: 2, iterations: 5 };
        case "L":
            return { vus: 3, iterations: 10 };
        default:
            throw new Error("Invalid 'Size' value: " + configuration.Size);
    }
}

export const options: Options = {
    vus: size().vus,
    thresholds: {
        http_req_duration: ["p(90)<160", "p(98)<190"]
    },
    iterations: size().iterations
};

export function setup(): Data {
    return { authToken: getAuthorizationHeader(configuration) };
}

export default function (data: Data): void {
    describe("Post a Token with Authentication:", () => {
        const bodyTokenContent = {
            content: "AAAA",
            expiresAt: tomorrow().toJSON().slice(0, 10)
        };

        const response = http.post(`${apiEndpoint}/Tokens`, JSON.stringify(bodyTokenContent), {
            headers: {
                Authorization: data.authToken,
                "Content-Type": "application/json"
            }
        });

        expect(response.status, "response status").to.equal(201);
    });
}
