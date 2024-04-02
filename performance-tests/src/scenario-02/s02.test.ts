import { SimpleLoggerFactory } from "@js-soft/simple-logger";
import { SyncRunType, TransportLoggerFactory } from "@nmshd/transport";
import { LogLevel } from "typescript-logging";
import { BackboneClient } from "../BackboneClient";

TransportLoggerFactory.init(new SimpleLoggerFactory(LogLevel.Fatal));

// maybe we should somehow honour the 1s rule when implementing k6.
(async () => {
    const config = {
        baseUrl: "http://localhost:8081",
        platformClientId: "test",
        platformClientSecret: "test"
    };

    const backboneClient1 = await BackboneClient.initWithNewIdentity(config);

    let syncRes = await backboneClient1.sync.startSyncRun({ type: SyncRunType.DatawalletVersionUpgrade, duration: 20 });

    // unsure whether we should call this or not.
    await backboneClient1.sync.finalizeDatawalletVersionUpgrade(syncRes.value.syncRun?.id!, { newDatawalletVersion: 1, datawalletModifications: [] });
})();
