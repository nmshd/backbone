import { SimpleLoggerFactory } from "@js-soft/simple-logger";
import { TransportLoggerFactory } from "@nmshd/transport";
import { DateTime } from "luxon";
import { LogLevel } from "typescript-logging";
import { BackboneClient } from "./BackboneClient";

TransportLoggerFactory.init(new SimpleLoggerFactory(LogLevel.Fatal));

const config = {
    baseUrl: "http://localhost:8081",
    platformClientId: "test",
    platformClientSecret: "test"
};

(async () => {
    const backboneClient1 = await BackboneClient.initWithNewIdentity(config);

    const template1 = await backboneClient1.relationshipTemplates.createRelationshipTemplate({ content: "AAAA", expiresAt: DateTime.utc().plus({ days: 1 }).toISO() });

    const backboneClient2 = await backboneClient1.createNewClientForNewDevice();

    const template2 = await backboneClient2.relationshipTemplates.createRelationshipTemplate({ content: "BBBB", expiresAt: DateTime.utc().plus({ days: 1 }).toISO() });

    console.log(template1);
    console.log(template2);
})();
