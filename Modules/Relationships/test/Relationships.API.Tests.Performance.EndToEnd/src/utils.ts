import http from "k6/http";
import exec from "k6/execution";

export interface Configuration {
  host: string;
  clientSecret: string;
  user: string;
  password: string;
  size: string;
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
    client_secret: configuration.clientSecret,
    username: configuration.user,
    password: configuration.password,
    grant_type: "password",
  };

  const authToken = http
    .post(`${configuration.host}/connect/token`, bodyConnectToken, {
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
    host: simplifyHost(__ENV.HOST),
    clientSecret: __ENV.CLIENT_SECRET,
    user: __ENV.USER,
    password: __ENV.PASSWORD,
    size: __ENV.SIZE,
  };
}

function assertEnvVarExists(parameter: string) {
  if (!__ENV[parameter]) {
    exec.test.abort(`Parameter '${parameter}' cannot be null or empty`);
  }
}

function simplifyHost(host: string): string {
  if (host.endsWith("/")) {
    host = host.slice(0, -1);
  }

  return host;
}
