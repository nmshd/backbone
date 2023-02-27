import http from "k6/http";
import { Options } from "k6/options";
import {
  describe,
  expect,
} from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import { getConfiguration, getAuthenticationHeader } from "./utils";

getConfiguration();

const apiEndpoint = __ENV.HOST + "/api/v1";

interface Data {
  authToken: string;
  relationshipTemplateId: string;
}

interface Size {
  vus: number;
  iterations: number;
}

function size(): Size {
  switch (__ENV.SIZE) {
    case "S":
      return { vus: 1, iterations: 10 };
    case "M":
      return { vus: 10, iterations: 50 };
    case "L":
      return { vus: 50, iterations: 100 };
    default:
      throw new Error("Invalid 'Size' value: " + __ENV.SIZE);
  }
}

export const options: Options = {
  vus: size().vus,
  thresholds: {
    http_req_duration: ["p(90)<50", "p(98)<100"],
  },
  iterations: size().iterations,
};

export function setup(): Data {
  const authToken = `Bearer ${getAuthenticationHeader()}`;

  const body = {
    maxNumberOfAllocations: 1,
    expiresAt: "2023-06-07T15:31:56.129Z",
    content: "AAAA",
  };
  const relationshipTemplateId = http
    .post(`${apiEndpoint}/RelationshipTemplates`, JSON.stringify(body), {
      headers: {
        "Content-Type": "application/json",
        Authorization: authToken,
      },
    })
    .json("result.id")!
    .toString();

  return {
    authToken: authToken,
    relationshipTemplateId: relationshipTemplateId,
  };
}

export default function (data: Data): void {
  describe("Get a Relationship Template by Id:", () => {
    const response = http.get(
      `${apiEndpoint}/RelationshipTemplates/${data.relationshipTemplateId}`,
      {
        headers: {
          Authorization: data.authToken,
        },
      }
    );
    expect(response.status, "response status").to.equal(200);
  });
}
