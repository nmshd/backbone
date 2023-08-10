import { HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { INGXLoggerMetadata, NGXLoggerServerService, NgxLoggerLevel } from "ngx-logger";

@Injectable({
    providedIn: "root"
})
export class LoggerServerService extends NGXLoggerServerService {
    protected override alterHttpRequest(httpRequest: HttpRequest<any>): HttpRequest<any> {
        httpRequest = httpRequest.clone({
            setHeaders: {
                ["X-API-KEY"]: localStorage.getItem("api-key")!
            }
        });
        return httpRequest;
    }

    public customiseRequestBody(metadata: INGXLoggerMetadata) {
        return {
            logLevel: metadata.level,
            category: metadata.fileName,
            messageTemplate: metadata.message,
            arguments: metadata.additional
        } as LoggerRequest;
    }
}

export interface LoggerRequest {
    logLevel: NgxLoggerLevel;
    category: string;
    messageTemplate: string;
    arguments: any[];
}
