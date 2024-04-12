import { SimpleLoggerFactory } from "@js-soft/simple-logger";
import { SyncRunType, TransportLoggerFactory } from "@nmshd/transport";
import pLimit, { LimitFunction } from "p-limit";
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

        // TODO: implement k6 checks for the received changes
    }

    console.log("finished run");
};

function divideByTwoUntilSmallerThan(seconds: number, threshold: number, ceil = false): number[] {
    const result: number[] = [];
    let current = seconds;

    while (current > threshold) {
        result.push(current);
        current /= 2;
        if (ceil) {
            current = Math.ceil(current);
        }
    }
    return result.reverse();
}

function distributeAmongBags(p: number, b: number): number[] | null {
    if (p <= 0 || b <= 0) return null; // Invalid input

    const distribution: number[] = [];
    let remaining = p;
    let currentAmount = p;

    // Calculate the amount to be distributed in each bag
    for (let i = 0; i < b; i++) {
        const amountInBag = Math.ceil(currentAmount / 2 ** (b - i - 1));
        if (amountInBag <= 0) return null; // Invalid distribution
        distribution.push(amountInBag);
        remaining -= amountInBag;
        currentAmount = amountInBag; // Update current amount for next bag
    }

    return distribution;
}

/**
 *
 * @param totalSteps number of times to call the function
 * @param totalTime time in seconds
 */
const createTasks = (totalSteps: number, totalTime: number): Map<number, LimitFunction> => {
    const timeSteps = divideByTwoUntilSmallerThan(totalTime, 2);
    const stepsCount = distributeAmongBags(totalSteps, timeSteps.length);
    const ret = new Map<number, LimitFunction>();

    console.log(timeSteps, stepsCount);
    let i = 0;
    timeSteps.forEach((timeStep) => {
        ret.set(timeStep, pLimit(stepsCount![i++]));
    });

    return ret;
};

(async () => {
    // const limit = pLimit(5);

    // let calls: Promise<void>[] = [];
    // for (let i = 0; i < 30; i++) {
    //     calls.push(limit(() => main()));
    // }
    // let a = setInterval(() => {
    //     console.log(limit.activeCount, limit.pendingCount);
    // }, 500);

    // await Promise.all(calls).then(() => console.log("done"));
    // clearTimeout(a);
    let a = createTasks(50, 600);

    console.log(a);
})();
