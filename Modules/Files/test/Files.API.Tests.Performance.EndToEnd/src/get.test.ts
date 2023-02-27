import http from "k6/http";
import { Options } from "k6/options";
import {
  describe,
  expect,
} from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import { getAuthenticationHeader, assertEnvVarExists, Size } from "./utils";

assertEnvVarExists();

const apiEndpoint = __ENV.HOST + "/api/v1";

interface Data {
  authToken: string;
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
  return { authToken: getAuthenticationHeader() };
}

export default function (data: Data): void {
  describe("Get list of Files:", () => {
    const response = http.get(`${apiEndpoint}/Files`, {
      headers: {
        Authorization: data.authToken,
      },
    });

    expect(response.status, "response status").to.equal(200);
  });
}
