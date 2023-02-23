import http from "k6/http";
import { Options } from "k6/options";
import {
  describe,
  expect,
} from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import { assertEnvVarExists, getJwt } from "./utils";

assertEnvVarExists();

const apiEndpoint = __ENV.HOST + "/api/v1";

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
  return getJwt();
}

export default function (authToken: string): void {
  describe("Post a Challenge with Authentication:", () => {
    const response = http.post(`${apiEndpoint}/Challenges`, null, {
      headers: {
        Authorization: `Bearer ${authToken}`,
      },
    });
    expect(response.status, "response status").to.equal(201);
  });
}
