import { Inject, Injectable, PLATFORM_ID } from "@angular/core";
import { INGXLoggerConfig, INGXLoggerMetadata, NGXLoggerWriterService, NgxLoggerLevel } from "ngx-logger";

@Injectable({
    providedIn: "root"
})
export class LoggerWriterService extends NGXLoggerWriterService {
    constructor(@Inject(PLATFORM_ID) protected platformId: any) {
        super(platformId);
    }

    protected override logIE(metadata: INGXLoggerMetadata, config: INGXLoggerConfig, metaString: string): void {
        const additional = metadata.additional || [];

        switch (metadata.level) {
            case NgxLoggerLevel.WARN:
                console.warn(`${metaString} ` + metadata.message, ...additional);
                break;
            case NgxLoggerLevel.ERROR:
            case NgxLoggerLevel.FATAL:
                console.error(`${metaString} ` + metadata.message, ...additional);
                break;
            case NgxLoggerLevel.INFO:
                console.info(`${metaString} ` + metadata.message, ...additional);
                break;
            default:
                console.log(`${metaString} ` + metadata.message, ...additional);
        }
    }

    protected override logModern(metadata: INGXLoggerMetadata, config: INGXLoggerConfig, metaString: string): void {
        const color = this.getColor(metadata, config);
        const additional = metadata.additional || [];

        switch (metadata.level) {
            case NgxLoggerLevel.WARN:
                console.warn(`%c${metaString}` + metadata.message, `color:${color}`, ...additional);
                break;
            case NgxLoggerLevel.ERROR:
            case NgxLoggerLevel.FATAL:
                console.error(`%c${metaString}` + metadata.message, `color:${color}`, ...additional);
                break;
            case NgxLoggerLevel.INFO:
                console.info(`%c${metaString}` + metadata.message, `color:${color}`, ...additional);
                break;
            case NgxLoggerLevel.DEBUG:
                console.debug(`%c${metaString}` + metadata.message, `color:${color}`, ...additional);
                break;
            default:
                console.log(`%c${metaString}` + metadata.message, `color:${color}`, ...additional);
        }
    }
}
