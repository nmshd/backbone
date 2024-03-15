import { Injectable } from "@angular/core";

@Injectable({
    providedIn: "root"
})
export class DeletionProcessService {
    private readonly deletionProcessMockData: DeletionProcess[] = [
        {
            id: "id1",
            status: 0,
            createdAt: "2024-02-15T10:30:00Z",
            approvalReminder1SentAt: "2024-02-16T10:30:00Z",
            approvalReminder2SentAt: "2024-02-17T10:30:00Z",
            approvalReminder3SentAt: "2024-02-19T10:30:00Z",
            approvedAt: "",
            approvedByDevice: "DVC4mhsDjf1rgJCUPmME",
            gracePeriodEndsAt: "2024-03-01T10:00:00Z",
            gracePeriodReminder1SentAt: "2024-03-01T10:30:00Z",
            gracePeriodReminder2SentAt: "2024-03-05T10:30:00Z",
            gracePeriodReminder3SentAt: "2024-03-09T10:30:00Z",
            identityAddress: "id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm"
        },
        {
            id: "id2",
            status: 1,
            createdAt: "2024-02-15T10:30:00Z",
            approvalReminder1SentAt: "2024-02-16T10:30:00Z",
            approvalReminder2SentAt: "2024-02-17T10:30:00Z",
            approvalReminder3SentAt: "2024-02-18T10:30:00Z",
            approvedAt: "2024-02-18T15:20:00Z",
            approvedByDevice: "DVC4mhsDjf1rgJCUPmME",
            gracePeriodEndsAt: "2024-03-05T10:30:00Z",
            gracePeriodReminder1SentAt: "2024-03-02T10:30:00Z",
            gracePeriodReminder2SentAt: "2024-03-03T10:30:00Z",
            gracePeriodReminder3SentAt: "2024-03-04T10:30:00Z",
            identityAddress: "id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm"
        }
    ];

    private readonly deletionProcessAuditLogMockData: DeletionProcessAuditLog[] = [
        {
            id: "id1",
            createdAt: "2024-02-18T15:20:00Z",
            message: "Some message for deletion process audit log one.",
            identityAddressHash: "e8dc4081b13434b45189a720b77",
            deviceIdHash: "c95a9e94c94e5771781064be501da227ba",
            newStatus: 0,
            identityDeletionProcessId: "id1"
        },
        {
            id: "id1",
            createdAt: "2024-02-18T15:20:00Z",
            message: "Some message for deletion process audit log one.",
            identityAddressHash: "e8dc4081b13434b45189a720b77",
            deviceIdHash: "c95a9e94c94e5771781064be501da227ba",
            oldStatus: 0,
            newStatus: 1,
            identityDeletionProcessId: "id1"
        },
        {
            id: "id2",
            createdAt: "2024-02-18T15:20:00Z",
            message: "Some message for deletion process audit log two.",
            identityAddressHash: "2a8ad718c57224bc81beae0b6d7a04",
            deviceIdHash: "7dd52d8157f28bafa4aebb9b3bcb82e9c685",
            oldStatus: 1,
            newStatus: 2,
            identityDeletionProcessId: "id2"
        }
    ];

    public getStatuses(): string[] {
        return ["Waiting for Approval", "Approved", "Deleting", "Rejected", "Cancelled"];
    }

    public getDeletionProcessMockData(): DeletionProcess[] {
        return this.deletionProcessMockData;
    }

    public getDeletionProcessAuditLogMockData(): DeletionProcessAuditLog[] {
        return this.deletionProcessAuditLogMockData;
    }
}

export interface DeletionProcess {
    id: string;
    status: number;
    createdAt: string;
    approvalReminder1SentAt?: string;
    approvalReminder2SentAt?: string;
    approvalReminder3SentAt?: string;
    approvedAt: string;
    approvedByDevice: string;
    gracePeriodEndsAt: string;
    gracePeriodReminder1SentAt?: string;
    gracePeriodReminder2SentAt?: string;
    gracePeriodReminder3SentAt?: string;
    identityAddress: string;
}

export interface DeletionProcessAuditLog {
    id: string;
    createdAt: string;
    message: string;
    identityAddressHash: string;
    deviceIdHash: string;
    oldStatus?: number;
    newStatus: number;
    identityDeletionProcessId: string;
}
