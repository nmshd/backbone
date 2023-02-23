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
  fileId: string;
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

  const bodyFileContent = {
    content: http.file(
      "For performance testing purposes.",
      "PerformanceTest.txt"
    ),
    cipherHash: "AAAA",
    expiresAt: tomorrow().toJSON().slice(0, 10),
    encryptedProperties: "AAAA",
  };

  const fileId = http
    .post(`${apiEndpoint}/Files`, bodyFileContent, {
      headers: {
        Authorization: `Bearer ${authToken}`,
      },
    })
    .json("result.id")!
    .toString();

  return {
    authToken: authToken,
    fileId: fileId,
  };
}

export default function (data: Data): void {
  describe("Get a File metadata:", () => {
    const response = http.get(`${apiEndpoint}/Files/${data.fileId}/metadata`, {
      headers: {
        Authorization: `Bearer ${data.authToken}`,
      },
    });

    expect(response.status, "response status").to.equal(200);
  });
}

function tomorrow() {
  const date = new Date();
  date.setDate(date.getDate() + 1);

  return date;
}
