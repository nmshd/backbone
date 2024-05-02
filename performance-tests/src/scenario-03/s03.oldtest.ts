import { SimpleLoggerFactory } from "@js-soft/simple-logger";
import { BackboneExternalEvent, SyncRunType, TransportLoggerFactory } from "@nmshd/transport";
import { LogLevel } from "typescript-logging";
import { BackboneClient } from "../BackboneClient";
import { generateDataWalletModifications, randomBytesAsBase64String, sleep } from "../utils";

TransportLoggerFactory.init(new SimpleLoggerFactory(LogLevel.Fatal));

// maybe we should somehow honour the 1s rule when implementing k6.
(async () => {
    const config = {
        baseUrl: "http://localhost:8081",
        platformClientId: "test",
        platformClientSecret: "test"
    };

    const connector = await BackboneClient.initWithNewIdentity(config);
    let userLocalDatawalletModificationsIndex = -1;
    let connectorLocalDatawalletModificationsIndex = 1;

    const connectorRelationshipTemplate = await connector.relationshipTemplates.createRelationshipTemplate({
        content: randomBytesAsBase64String(64), // base64 encoded string
        expiresAt: new Date(new Date().getTime() + 3600 * 24 * 1000).toDateString(),
        maxNumberOfAllocations: 10 // TODO check this number
    });

    //sleep(60);

    const user = await BackboneClient.initWithNewIdentity(config);

    let syncRes = await user.sync.startSyncRun({ type: SyncRunType.DatawalletVersionUpgrade, duration: 10 });

    let upgrade = await user.sync.finalizeDatawalletVersionUpgrade(syncRes.value.syncRun?.id!, { newDatawalletVersion: 1, datawalletModifications: generateDataWalletModifications(40, 300) });
    userLocalDatawalletModificationsIndex = (upgrade.value as any).newDatawalletModificationIndex; // Todo remove cast when PR is merged and package is updated

    //sleep(5);

    const connectorRelationshipTemplateFetchedByUser = (await user.relationshipTemplates.getRelationshipTemplate(connectorRelationshipTemplate.value.id)).value;

    await user.sync.createDatawalletModifications({ modifications: generateDataWalletModifications(5, 300) });

    //sleep(60);

    const userCreatedRelationship = (
        await user.relationships.createRelationship({
            relationshipTemplateId: connectorRelationshipTemplateFetchedByUser.id,
            content: randomBytesAsBase64String(1024)
        })
    ).value;

    let createRes = await user.sync.createDatawalletModifications({ localIndex: userLocalDatawalletModificationsIndex, modifications: generateDataWalletModifications(5, 300) });

    userLocalDatawalletModificationsIndex = createRes.value.newIndex;

    //sleep(2);

    syncRes = await connector.sync.startSyncRun({ type: SyncRunType.DatawalletVersionUpgrade, duration: 10 });
    await connector.sync.finalizeDatawalletVersionUpgrade(syncRes.value.syncRun?.id!, {
        newDatawalletVersion: connectorLocalDatawalletModificationsIndex,
        datawalletModifications: generateDataWalletModifications(5, 300)
    });

    await sleep(0.5);

    syncRes = await connector.sync.startSyncRun({ type: SyncRunType.ExternalEventSync, duration: 10 });
    // k6 check (await res.collect()).length == 1
    var relationshipChangeCreated = (await (await connector.sync.getExternalEventsOfSyncRun(syncRes.value.syncRun!.id)).value.collect()).at(0) as
        | (BackboneExternalEvent & { payload: { relationshipId: string; changeId: string } })
        | undefined;

    let b = (await connector.sync.finalizeExternalEventSync(syncRes.value.syncRun!.id, { externalEventResults: [], datawalletModifications: [] })).value;

    connectorLocalDatawalletModificationsIndex = b.newDatawalletModificationIndex;

    // k6 check relationshipChangeCreated?.payload.relationshipId == userCreatedRelationship.id;

    //sleep(3); // simulates a customer system that has to make the decision
    await connector.relationships.acceptRelationshipChange(relationshipChangeCreated?.payload.relationshipId!, relationshipChangeCreated?.payload.changeId!);

    await sleep(5);

    syncRes = await user.sync.startSyncRun({ type: SyncRunType.ExternalEventSync, duration: 10 });

    let relationshipChangeCompleted = (await (await user.sync.getExternalEventsOfSyncRun(syncRes.value.syncRun!.id)).value.collect()).at(0) as
        | (BackboneExternalEvent & { payload: { relationshipId: string; changeId: string } })
        | undefined;

    // k6 check relationshipChangeCreated?.payload.relationshipId == relationshipChangeCompleted?.payload.relationshipId;
    await user.sync.finalizeExternalEventSync(syncRes.value.syncRun?.id!, { datawalletModifications: [], externalEventResults: [] });

    await user.relationships.getRelationship(relationshipChangeCompleted?.payload.relationshipId!); // TODO why do we do this? (next to last step of the scenario)

    let a = await user.sync.createDatawalletModifications({ localIndex: userLocalDatawalletModificationsIndex, modifications: generateDataWalletModifications(5, 300) });
})();
