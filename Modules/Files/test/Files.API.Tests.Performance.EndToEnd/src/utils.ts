import http from "k6/http";
import exec from "k6/execution";

export interface Configuration {
  Host: string;
  Client_Secret: string;
  Username: string;
  Password: string;
  Size: string;
}

export interface Size {
  vus: number;
  iterations: number;
}

export function tomorrow(): Date {
  const date = new Date();
  date.setDate(date.getDate() + 1);

  return date;
}

export function getAuthenticationHeader(configuration: Configuration): string {
  const bodyConnectToken = {
    client_id: "test",
    client_secret: configuration.Client_Secret,
    username: configuration.Username,
    password: configuration.Password,
    grant_type: "password",
  };

  const authToken = http
    .post(`${configuration.Host}/connect/token`, bodyConnectToken, {
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
    })
    .json("access_token")!
    .toString();

  return `Bearer ${authToken}`;
}

export function getConfiguration(): Configuration {
  assertAllRequiredEnvVarsExist();

  return {
    Host: __ENV.HOST,
    Client_Secret: __ENV.CLIENT_SECRET,
    Username: __ENV.USERNAME,
    Password: __ENV.PASSWORD,
    Size: __ENV.SIZE,
  };
}

export function assertAllRequiredEnvVarsExist() {
  if (!__ENV.HOST) {
    exec.test.abort("Parameter 'HOST' cannot be null or empty");
  }

  if (!__ENV.CLIENT_SECRET) {
    exec.test.abort("Parameter 'CLIENT_SECRET' cannot be null or empty");
  }

  if (!__ENV.USERNAME) {
    exec.test.abort("Parameter 'USERNAME' cannot be null or empty");
  }

  if (!__ENV.PASSWORD) {
    exec.test.abort("Parameter 'PASSWORD' cannot be null or empty");
  }

  if (!__ENV.SIZE) {
    exec.test.abort("Parameter 'SIZE' cannot be null or empty");
  }
}
