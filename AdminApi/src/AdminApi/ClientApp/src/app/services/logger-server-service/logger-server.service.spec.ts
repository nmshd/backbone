import { TestBed } from "@angular/core/testing";

import { LoggerServerService } from "./logger-server.service";

describe("LoggerServerService", function () {
    let service: LoggerServerService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(LoggerServerService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
