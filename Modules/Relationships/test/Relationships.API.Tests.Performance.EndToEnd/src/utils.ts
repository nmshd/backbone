import http from "k6/http";
import exec from "k6/execution";

export function getJwt() {
  const bodyConnectToken = {
    client_id: "test",
    client_secret: __ENV.CLIENT_SECRET,
    username: __ENV.USERNAME,
    password: __ENV.PASSWORD,
    grant_type: "password",
  };

  const authToken = http
    .post(`${__ENV.HOST}/connect/token`, bodyConnectToken, {
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
    })
    .json("access_token")!
    .toString();

  return authToken;
}

export function assertEnvVarExists() {
  if (!__ENV.HOST) {
    exec.test.abort("Invalid 'HOST' parameter value");
  }

  if (!__ENV.CLIENT_SECRET) {
    exec.test.abort("Invalid 'CLIENT_SECRET' parameter value");
  }

  if (!__ENV.USERNAME) {
    exec.test.abort("Invalid 'USERNAME' parameter value");
  }

  if (!__ENV.PASSWORD) {
    exec.test.abort("Invalid 'PASSWORD' parameter value");
  }

  if (!__ENV.SIZE) {
    exec.test.abort("Invalid 'SIZE' parameter value");
  }
}