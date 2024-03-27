import { SimpleLoggerFactory } from "@js-soft/simple-logger";
import { SyncRunType, TransportLoggerFactory } from "@nmshd/transport";
import { LogLevel } from "typescript-logging";
import { BackboneClient } from "../BackboneClient";
import { randomIntFromInterval } from "../utils";

TransportLoggerFactory.init(new SimpleLoggerFactory(LogLevel.Fatal));

(async () => {
    const config = {
        baseUrl: "http://localhost:8081",
        platformClientId: "test",
        platformClientSecret: "test"
    };

    const backboneClient1 = await BackboneClient.initWithNewIdentity(config);
    const backboneClient2 = await backboneClient1.createNewClientForNewDevice();
    const backboneClient3 = await backboneClient1.createNewClientForNewDevice();

    for (let i = 0; i < randomIntFromInterval(10, 15); i++) {
        // TODO: check if the duration is sound
        let syncRes = await backboneClient1.sync.startSyncRun({ type: SyncRunType.DatawalletVersionUpgrade, duration: 2000 });

        // TODO: ajust the payload to 300B
        await backboneClient1.sync.finalizeDatawalletVersionUpgrade(syncRes.value.syncRun?.id!, {
            newDatawalletVersion: 1,
            datawalletModifications: [{ objectIdentifier: "test", collection: "Messages", type: "Create", encryptedPayload: "encryptedPayload", datawalletVersion: 1 }]
        });

        await Promise.all([backboneClient2.sync.startSyncRun(), backboneClient3.sync.startSyncRun()]);

        (await backboneClient2.sync.getDatawalletModifications({ localIndex: -100 })).value.collect();
        (await backboneClient3.sync.getDatawalletModifications({ localIndex: -100 })).value.collect();

        // TODO: implement k6 checks for the received changes
    }
})();
