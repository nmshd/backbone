import { HttpBackend, HttpRequest } from "@angular/common/http";
import { Injectable, NgZone } from "@angular/core";
import { INGXLoggerMetadata, NGXLoggerServerService, NgxLoggerLevel } from "ngx-logger";
import { AuthService } from "../auth-service/auth.service";
import { Observable } from "rxjs";

@Injectable({
    providedIn: "root"
})
export class LoggerServerService extends NGXLoggerServerService {
    isLoggedIn$: Observable<boolean> | undefined;

    constructor(httpBackend: HttpBackend, ngZone: NgZone, private authService: AuthService) {
        super(httpBackend, ngZone);
    }

    protected override alterHttpRequest(httpRequest: HttpRequest<any>): HttpRequest<any> {
        this.isLoggedIn$ = this.authService.isLoggedIn;
        if (this.isLoggedIn$) {
            httpRequest = httpRequest.clone({
                setHeaders: {
                    "X-API-KEY": this.authService.getApiKey()!
                }
            });
        }

        return httpRequest;
    }

    public override customiseRequestBody(metadata: INGXLoggerMetadata) {
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
