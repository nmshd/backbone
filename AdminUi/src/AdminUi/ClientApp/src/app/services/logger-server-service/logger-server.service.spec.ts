import { TestBed } from "@angular/core/testing";

import { LoggerServerService } from "./logger-server.service";

describe("LoggerServerService", () => {
    let service: LoggerServerService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(LoggerServerService);
    });

    it("should be created", () => {
        expect(service).toBeTruthy();
    });
});
