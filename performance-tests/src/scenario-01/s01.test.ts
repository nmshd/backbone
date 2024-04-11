import { SimpleLoggerFactory } from "@js-soft/simple-logger";
import { SyncRunType, TransportLoggerFactory } from "@nmshd/transport";
import pLimit from "p-limit";
import { LogLevel } from "typescript-logging";
import { BackboneClient } from "../BackboneClient";
import { generateDataWalletModifications, randomIntFromInterval } from "../utils";

TransportLoggerFactory.init(new SimpleLoggerFactory(LogLevel.Fatal));

let main = async () => {
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
            datawalletModifications: generateDataWalletModifications(1, 300)
        });

        await Promise.all([backboneClient2.sync.startSyncRun(), backboneClient3.sync.startSyncRun()]);

        (await backboneClient2.sync.getDatawalletModifications({ localIndex: -100 })).value.collect();
        (await backboneClient3.sync.getDatawalletModifications({ localIndex: -100 })).value.collect();

        console.log("finished run");
        // TODO: implement k6 checks for the received changes
    }
};
(async () => {
    const limit = pLimit(10); // only 10 at once

    let calls: Promise<void>[] = [];
    for (let i = 0; i < 8; i++) {
        calls.push(limit(() => main()));
    }
    setInterval(() => {
        console.log(limit.activeCount, limit.pendingCount);
    }, 500);

    let res = await Promise.all(calls).then(() => console.log("done"));
    console.log(res);
})();
