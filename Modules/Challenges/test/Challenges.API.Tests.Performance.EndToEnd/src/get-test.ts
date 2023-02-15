import http from 'k6/http';
import { check } from 'k6';
import { Options } from 'k6/options';

const host = __ENV.HOST //http://localhost:5000;
const apiEndpoint = host+'/api/v1/';
const bodyConnectToken = {
    client_id: 'test',
    client_secret: 'test',
    username: 'USRa',
    password: 'a',
    grant_type: 'password'
};

export interface Data {
    authToken: string,
    challengesId: string
}

export let options: Options = {
    vus: 1,
    thresholds: {
        http_req_duration: ['p(90)<50', 'p(98)<100'],// 90% of requests should be below 50ms| // 100% of requests should be below 100ms
    },
    iterations: 100
};

export function setup () {
    const getConnectToken = http.post(
        `${host}/connect/token`,
        bodyConnectToken,
        {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }
    );

    const authToken = getConnectToken.json('access_token')!.toString();

    const postChallengesWithAuthentication = http.post(
        `${apiEndpoint}Challenges`,
        null,
        {
            headers: {
                    'Authorization': `Bearer ${authToken}`,
            }
        }
    );

    const challengesId = postChallengesWithAuthentication.json('result.id');

    return { authToken: authToken, challengesId: challengesId}
}


export default function ( data: Data): void {

    const getChallengesId = http.get(
        `${apiEndpoint}Challenges/${data.challengesId}`,
        {
            headers: {
                'Authorization': `Bearer ${data.authToken}`,
        }
        }
    );

    check(getChallengesId, {
        'Expected status is 200': (r) => r.status == 200,
    });

};


