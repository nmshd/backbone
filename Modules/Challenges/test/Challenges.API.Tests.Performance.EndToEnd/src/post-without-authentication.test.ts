import http from 'k6/http';
import { Options } from 'k6/options';
import { describe, expect } from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";

const host = __ENV.HOST;
const apiEndpoint = host+'/api/v1/';

function translateSize() {
    switch(__ENV.SIZE){
        case "S": return { vus: 1, iterations: 10 };
        case "M": return { vus: 10, iterations: 50 };
        case "L": return { vus: 50, iterations: 100 };
        default: throw new Error("Invalid 'Size' value");
    }
}

export const options: Options = {
    vus: translateSize().vus,
    thresholds: {
        http_req_duration: ['p(90)<50', 'p(98)<100'],
    },
    iterations: translateSize().iterations
};

export default function (): void {   
    describe("Post a Challenge without Authentication:", () => {
        const response = http.post(
            `${apiEndpoint}Challenges`,
            null
        );
        expect(response.status, "response status").to.equal(201);
    });
};


