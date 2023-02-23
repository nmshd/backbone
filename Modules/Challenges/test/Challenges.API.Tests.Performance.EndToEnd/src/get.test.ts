import http from "k6/http";
import { Options } from "k6/options";
import {
  describe,
  expect,
} from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import { getJwt, assertEnvVarExists } from "./utils";

assertEnvVarExists();

const apiEndpoint = __ENV.HOST + "/api/v1";

interface Data {
  authToken: string;
  challengeId: string;
}

function size() {
  switch (__ENV.SIZE) {
    case "S":
      return { vus: 1, iterations: 10 };
    case "M":
      return { vus: 10, iterations: 50 };
    case "L":
      return { vus: 50, iterations: 100 };
    default:
      throw new Error("Invalid 'Size' value");
  }
}

export const options: Options = {
  vus: size().vus,
  thresholds: {
    http_req_duration: ["p(90)<50", "p(98)<100"],
  },
  iterations: size().iterations,
};

export function setup() {
  const authToken = getJwt();

  const challengeId = http
    .post(`${apiEndpoint}/Challenges`)
    .json("result.id")!
    .toString();

  return {
    authToken: authToken,
    challengeId: challengeId,
  };
}

export default function (data: Data): void {
  describe("Get a Challenge:", () => {
    const response = http.get(`${apiEndpoint}/Challenges/${data.challengeId}`, {
      headers: {
        Authorization: `Bearer ${data.authToken}`,
      },
    });
    expect(response.status, "response status").to.equal(200);
  });
}
