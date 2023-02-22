import http from 'k6/http';
import { Options } from 'k6/options';
import { describe, expect } from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";

const host = __ENV.HOST;
const apiEndpoint = host+'/api/v1/';

interface Data {
    authToken: string,
    relationshipTemplateId: string
}

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

export function setup () {
    const bodyConnectToken = {
        client_id: 'test',
        client_secret: __ENV.CLIENT_SECRET,
        username: __ENV.USERNAME,
        password: __ENV.PASSWORD,
        grant_type: 'password'
    };
    const authToken = http.post(
        `${host}/connect/token`,
        bodyConnectToken,
        {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }
    )
    .json("access_token")!
    .toString();

    const body = {
        "createdBy":"id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm",
        "createdByDevice": "DVCijQcf7seWPJ28NwGD",
        "maxNumberOfAllocations": 1,
        "expiresAt": "2023-06-07T15:31:56.129Z",
        "createdAt": "2023-01-07T15:31:56.129Z",
        "content": "AAAA"
    };
    const relationshipTemplateId = http.post(
        `${apiEndpoint}RelationshipTemplates`,
        JSON.stringify(body),
        {
            headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${authToken}`,
            }
        }
    )    
    .json("result.id")!
    .toString();

    return {
        authToken: authToken,
        relationshipTemplateId: relationshipTemplateId
    }
}


export default function (data: Data): void {
    describe("Get a Relationship Template by Id:", () => {
        const response = http.get(
            `${apiEndpoint}RelationshipTemplates/${data.relationshipTemplateId}`,
            {
                headers: {
                    'Authorization': `Bearer ${data.authToken}`,
                }
            }
        );
        expect(response.status,"response status").to.equal(200);
    });
};


