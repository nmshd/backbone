import http from "k6/http";
import { Options } from "k6/options";
import {
  describe,
  expect,
} from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import {
  getAuthorizationHeader,
  getConfiguration,
  Size,
  tomorrow,
} from "./utils";

const configuration = getConfiguration();

const apiEndpoint = configuration.Host + "/api/v1";

interface Data {
  authToken: string;
  tokenId: string;
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
  const authToken = getAuthorizationHeader(configuration);

  const bodyTokenContent = {
    content: "AAAA",
    expiresAt: tomorrow().toJSON().slice(0, 10),
  };

  const tokenId = http
    .post(`${apiEndpoint}/Tokens`, JSON.stringify(bodyTokenContent), {
      headers: {
        Authorization: authToken,
        "Content-Type": "application/json",
      },
    })
    .json("result.id")!
    .toString();

  return {
    authToken: authToken,
    tokenId: tokenId,
  };
}

export default function (data: Data): void {
  describe("Get a Token:", () => {
    const response = http.get(`${apiEndpoint}/Tokens/${data.tokenId}`, {
      headers: {
        Authorization: data.authToken,
      },
    });

    expect(response.status, "response status").to.equal(200);
  });
}
