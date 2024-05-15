export interface StartSyncRunRequestBody {
    type: SyncRunType;
    duration: number;
}

export enum SyncRunType {
    ExternalEventSync,
    DatawalletVersionUpgrade
}

interface BackboneSyncRun {
    id: string;
    expiresAt: string;
    index: number;
    createdAt: string;
    createdBy: string;
    createdByDevice: string;
    eventCount: number;
}

export interface StartSyncRunResponse {
    status: StartSyncRunStatus;
    syncRun: BackboneSyncRun | null;
}

declare enum StartSyncRunStatus {
    Created = "Created",
    NoNewEvents = "NoNewEvents"
}
