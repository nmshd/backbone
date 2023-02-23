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
  tokenId: string;
}

interface Size {
  vus: number;
  iterations: number;
}

function size(): Size {
  switch (__ENV.SIZE) {
    case "S":
      return { vus: 1, iterations: 10 } as Size;
    case "M":
      return { vus: 10, iterations: 50 } as Size;
    case "L":
      return { vus: 50, iterations: 100 } as Size;
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
  const authToken = getJwt();

  const bodyTokenContent = {
    content: "123",
    expiresAt: tomorrow().toJSON().slice(0, 10),
  };

  const tokenId = http
    .post(`${apiEndpoint}/Tokens`, bodyTokenContent, {
      headers: {
        Authorization: `Bearer ${authToken}`,
      },
    })
    .json("result.id")!
    .toString();

  return {
    authToken: authToken,
    tokenId: tokenId,
  } as Data;
}

export default function (data: Data): void {
  describe("Get list of Tokens:", () => {
    const response = http.get(`${apiEndpoint}/Tokens/${data.tokenId}`, {
      headers: {
        Authorization: `Bearer ${data.authToken}`,
      },
    });

    expect(response.status, "response status").to.equal(200);
  });
}

function tomorrow(): Date {
  const date = new Date();
  date.setDate(date.getDate() + 1);

  return date;
}
