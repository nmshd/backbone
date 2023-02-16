import http from 'k6/http';
import { Options } from 'k6/options';
import { describe, expect } from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";

const host = __ENV.HOST;
const apiEndpoint = host+'/api/v1/';

export let options: Options = {
    vus: Number(__ENV.VUS),
    thresholds: {
        http_req_duration: ['p(90)<50', 'p(98)<100'],
    },
    iterations: Number(__ENV.ITERATIONS)
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


