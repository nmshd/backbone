import { HttpBackend, HttpRequest } from "@angular/common/http";
import { Injectable, NgZone } from "@angular/core";
import { INGXLoggerMetadata, NGXLoggerServerService, NgxLoggerLevel } from "ngx-logger";
import { AuthService } from "../auth-service/auth.service";
import { Observable } from "rxjs";
import { XSRFService } from "../xsrf-service/xsrf.service";

@Injectable({
    providedIn: "root"
})
export class LoggerServerService extends NGXLoggerServerService {
    private isLoggedIn$: Observable<boolean> | undefined;

    public constructor(
        httpBackend: HttpBackend,
        ngZone: NgZone,
        private readonly authService: AuthService,
        private readonly xsrfService: XSRFService
    ) {
        super(httpBackend, ngZone);
    }

    protected override alterHttpRequest(httpRequest: HttpRequest<any>): HttpRequest<any> {
        const token = this.xsrfService.getStoredToken();
        this.isLoggedIn$ = this.authService.isLoggedIn;

        this.isLoggedIn$.subscribe((isLoggedIn) => {
            if (isLoggedIn) {
                httpRequest = httpRequest.clone({
                    withCredentials: true,
                    setHeaders: {
                        // eslint-disable-next-line @typescript-eslint/naming-convention
                        "X-API-KEY": this.authService.getApiKey()!,
                        // eslint-disable-next-line @typescript-eslint/naming-convention
                        "X-XSRF-TOKEN": token
                    }
                });
            }
        });

        return httpRequest;
    }

    protected override customiseRequestBody(metadata: INGXLoggerMetadata): LoggerRequest {
        let messageTemplate: string = metadata.message;
        for (let i = 0; i < metadata.additional!.length; i++) {
            messageTemplate = messageTemplate.replace(new RegExp("(?<!%)%[dsj]"), `{${i}}`);
        }

        return {
            logLevel: metadata.level,
            category: metadata.fileName,
            messageTemplate: messageTemplate,
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
