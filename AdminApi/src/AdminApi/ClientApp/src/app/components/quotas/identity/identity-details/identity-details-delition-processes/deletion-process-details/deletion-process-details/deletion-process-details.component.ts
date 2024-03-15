import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DeletionProcess, DeletionProcessAuditLog, DeletionProcessService } from "src/app/services/deletion-process-service/deletion-process.service";

@Component({
    selector: "app-deletion-process-details",
    templateUrl: "./deletion-process-details.component.html",
    styleUrl: "./deletion-process-details.component.css"
})
export class DeletionProcessDetailsComponent {
    public id: string;

    public header: string;
    public headerDeletionProcessAuditLog: string;
    public headerDeletionProcessAuditLogDescription: string;

    public loading: boolean;
    public deletionProcessesAuditLogTableDisplayedColumns: string[];

    public readonly mockDataDeletionProcesses: DeletionProcess[];
    public readonly mockDataDeletionProcessAuditLogs: DeletionProcessAuditLog[];

    public mockDataDeletionProcess?: DeletionProcess;
    public mockDataDeletionProcessAuditLog?: DeletionProcessAuditLog[];

    public statuses: string[];
    public status: string;

    public constructor(
        private readonly deletionProcess: DeletionProcessService,
        private readonly activatedRoute: ActivatedRoute
    ) {
        this.header = "Deletion Process Details";
        this.headerDeletionProcessAuditLog = "Deletion Process Audit Logs";
        this.headerDeletionProcessAuditLogDescription = "View deletion process audit logs for Identity.";
        this.loading = false;
        this.deletionProcessesAuditLogTableDisplayedColumns = ["id", "createdAt", "message", "identityAddressHash", "deviceIdHash", "oldStatus", "newStatus"];

        this.id = "";

        this.mockDataDeletionProcesses = deletionProcess.getDeletionProcessMockData();
        this.mockDataDeletionProcessAuditLogs = deletionProcess.getDeletionProcessAuditLogMockData();
        this.statuses = deletionProcess.getStatuses();
        this.status = "";
    }

    public ngOnInit(): void {
        this.activatedRoute.params.subscribe((params) => {
            this.id = params["id"];
        });
        this.loadDeletionProcess(this.id);
        this.loadDeletionProcessAuditLogs(this.id);
    }

    private loadDeletionProcess(id: string): void {
        this.mockDataDeletionProcess = this.mockDataDeletionProcesses.find((data: DeletionProcess) => data.id === id);
    }

    private loadDeletionProcessAuditLogs(id: string): void {
        this.mockDataDeletionProcessAuditLog = this.mockDataDeletionProcessAuditLogs.filter((data: DeletionProcessAuditLog) => data.identityDeletionProcessId === id);
    }

    public showStatusDescription(status: number): string {
        for (let i = 0; i < this.statuses.length; i++) {
            if (i === status) {
                return this.statuses[i];
            }
        }
        return "";
    }
}
