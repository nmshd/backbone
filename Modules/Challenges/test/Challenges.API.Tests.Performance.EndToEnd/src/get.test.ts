import http from "k6/http";
import { Options } from "k6/options";
import {
  describe,
  expect,
} from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import { getAuthenticationHeader, getConfiguration, Size } from "./utils";

const configuration = getConfiguration();

const apiEndpoint = configuration.Host + "/api/v1";

interface Data {
  authToken: string;
  challengeId: string;
}

function size(): Size {
  switch (configuration.Size) {
    case "S":
      return { vus: 1, iterations: 10 };
    case "M":
      return { vus: 10, iterations: 50 };
    case "L":
      return { vus: 50, iterations: 100 };
    default:
      throw new Error("Invalid 'Size' value: " + configuration.Size);
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
  const challengeId = http
    .post(`${apiEndpoint}/Challenges`)
    .json("result.id")!
    .toString();

  return {
    authToken: `Bearer ${getAuthenticationHeader(configuration)}`,
    challengeId: challengeId,
  };
}

export default function (data: Data): void {
  describe("Get a Challenge:", () => {
    const response = http.get(`${apiEndpoint}/Challenges/${data.challengeId}`, {
      headers: {
        Authorization: `${data.authToken}`,
      },
    });
    expect(response.status, "response status").to.equal(200);
  });
}
