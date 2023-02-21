import http from "k6/http";
import { Options } from "k6/options";
import { describe, expect } from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";

const host = __ENV.HOST;
const apiEndpoint = host + "/api/v1/";

export function getSizeVus(size: string){
    switch(size){
        case "S": return 1;
        case "M": return 1;
        case "L": return 1;
        default: throw new Error("Invalid 'Size' value");
    }
}

export function getSizeIterations(size: string){
    switch(size){
        case "S": return 1;
        case "M": return 5;
        case "L": return 10;
        default: throw new Error("Invalid 'Size' value");
    }
}

export const options: Options = {
    vus: getSizeVus(__ENV.SIZE),
    thresholds: {
        http_req_duration: ["p(90)<160", "p(98)<190"],
    },
    iterations: getSizeIterations(__ENV.SIZE)
};

export function setup() {
    const bodyConnectToken = {
        client_id: "test",
        client_secret: __ENV.CLIENT_SECRET,
        username: "USRa",
        password: "a",
        grant_type: "password"
    };

    const authToken = http.post(
        `${host}/connect/token`,
        bodyConnectToken,
        {
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            }
        }
    ).json("access_token")!
    .toString();

    return authToken;
}

export default function (authToken: string): void {
    describe("Upload a File with Authentication:", () => {
        const bodyFileContent = {
            content: http.file("For performance testing purposes.", "PerformanceTest.txt"),
            cipherHash: "AAAA",
            expiresAt: getDate().toJSON().slice(0, 10),
            encryptedProperties: "AAAA"
        }

        const response = http.post(
            `${apiEndpoint}Files`,
            bodyFileContent,
            {
                headers: {
                    "Authorization": `Bearer ${authToken}`,
                }
            }
        );

        expect(response.status, "response status").to.equal(201);
    });
};

export function getDate(){
    const date = new Date();
    date.setDate(date.getDate() + 1)

    return date;
}