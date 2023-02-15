import http from 'k6/http';
import { check } from 'k6';
import { Options } from 'k6/options';

const host = __ENV.HOST //http://localhost:5000;
const apiEndpoint = host+'/api/v1/';

export let options: Options = {
    vus: 1,
    thresholds: {
        http_req_duration: ['p(90)<50', 'p(98)<100'],// 90% of requests should be below 50ms| // 100% of requests should be below 100ms
    },
    iterations: 100
};

export default function (): void {    
    const postChallengesWithoutAuthentication = http.post(
        `${apiEndpoint}Challenges`,
        null,
        {
            headers: {
            }
        }
    );

    check(postChallengesWithoutAuthentication, {
        'Expected status is 201': (r) => r.status == 201,
    });

};


