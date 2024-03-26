import { SimpleLoggerFactory } from "@js-soft/simple-logger";
import { SyncRunType, TransportLoggerFactory } from "@nmshd/transport";
import { LogLevel } from "typescript-logging";
import { BackboneClient } from "./BackboneClient";

TransportLoggerFactory.init(new SimpleLoggerFactory(LogLevel.Fatal));

export const options = {
    vus: 1,
    stages: [
        { duration: "20s", target: 5 },
        { duration: "20s", target: 20 },
        { duration: "90s", target: 20 },
        { duration: "20s", target: 0 }
    ],
    thresholds: { http_req_duration: ["avg<100", "p(95)<200"] },
    noConnectionReuse: true
};

export default async function () {
    const config = {
        baseUrl: "http://localhost:8081",
        platformClientId: "test",
        platformClientSecret: "test"
    };

    const backboneClient1 = await BackboneClient.initWithNewIdentity(config);

    const backboneClient2 = await backboneClient1.createNewClientForNewDevice();
    const backboneClient3 = await backboneClient1.createNewClientForNewDevice();

    let syncRes = await backboneClient1.sync.startSyncRun({ type: SyncRunType.DatawalletVersionUpgrade, duration: 2000 });
    await backboneClient1.sync.finalizeDatawalletVersionUpgrade(syncRes.value.syncRun?.id!, {
        newDatawalletVersion: 1,
        datawalletModifications: [{ objectIdentifier: "test", collection: "Messages", type: "Create", encryptedPayload: "encryptedPayload", datawalletVersion: 1 }]
    });

    await Promise.all([backboneClient2.sync.startSyncRun(), backboneClient3.sync.startSyncRun()]);

    (await backboneClient2.sync.getDatawalletModifications({ localIndex: -100 })).value.collect();
    (await backboneClient3.sync.getDatawalletModifications({ localIndex: -100 })).value.collect();
}
