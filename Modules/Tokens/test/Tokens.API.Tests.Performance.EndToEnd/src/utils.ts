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

export function getAuthorizationHeader(configuration: Configuration): string {
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
  assertEnvVarExists("HOST");
  assertEnvVarExists("CLIENT_SECRET");
  assertEnvVarExists("USER");
  assertEnvVarExists("PASSWORD");
  assertEnvVarExists("SIZE");

  return {
    Host: __ENV.HOST,
    Client_Secret: __ENV.CLIENT_SECRET,
    Username: __ENV.USER,
    Password: __ENV.PASSWORD,
    Size: __ENV.SIZE,
  };
}

function assertEnvVarExists(parameter: string) {
  if (!__ENV[parameter]) {
    exec.test.abort(`Parameter '${parameter}' cannot be null or empty`);
  }
}
