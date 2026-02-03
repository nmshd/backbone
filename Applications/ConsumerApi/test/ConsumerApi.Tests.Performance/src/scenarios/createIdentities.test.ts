import { Options } from "k6/options";
import { UnauthenticatedClient } from "../libs/backbone-client/unauthenticated-client";
import { Configuration } from "./configuration";

export const options: Options = {
    scenarios: {
        constantRequestRate: {
            executor: "constant-vus",
            duration: "1m",
            vus: 1
        }
    }
};

const configuration = Configuration.load();
const client = new UnauthenticatedClient(configuration.httpClient);

export default function (): void {
    client.createIdentity(configuration.httpClient.clientId, configuration.httpClient.clientSecret, "Password123!");
}
