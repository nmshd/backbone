import http from "k6/http";
import { Options } from "k6/options";
import {
  describe,
  expect,
} from "https://jslib.k6.io/k6chaijs/4.3.4.2/index.js";
import { getJwt } from "./utils";

const host = __ENV.HOST;
const apiEndpoint = host + "/api/v1";

function translateSize() {
  switch (__ENV.SIZE) {
    case "S":
      return { vus: 1, iterations: 1 };
    case "M":
      return { vus: 2, iterations: 5 };
    case "L":
      return { vus: 3, iterations: 10 };
    default:
      throw new Error("Invalid 'Size' value");
  }
}

export const options: Options = {
  vus: translateSize().vus,
  thresholds: {
    http_req_duration: ["p(90)<160", "p(98)<190"],
  },
  iterations: translateSize().iterations,
};

export function setup() {
  return getJwt();
}

export default function (authToken: string): void {
  describe("Upload a File with Authentication:", () => {
    const bodyFileContent = {
      content: http.file(
        "For performance testing purposes.",
        "PerformanceTest.txt"
      ),
      cipherHash: "AAAA",
      expiresAt: tomorrow().toJSON().slice(0, 10),
      encryptedProperties: "AAAA",
    };

    const response = http.post(`${apiEndpoint}/Files`, bodyFileContent, {
      headers: {
        Authorization: `Bearer ${authToken}`,
      },
    });

    expect(response.status, "response status").to.equal(201);
  });
}

function tomorrow() {
  const date = new Date();
  date.setDate(date.getDate() + 1);

  return date;
}
