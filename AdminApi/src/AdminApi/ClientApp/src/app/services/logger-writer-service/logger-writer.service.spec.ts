import { TestBed } from "@angular/core/testing";

import { LoggerWriterService } from "./logger-writer.service";

describe("LoggerWriterService", function () {
    let service: LoggerWriterService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(LoggerWriterService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
