import { TestBed } from "@angular/core/testing";

import { DeletionProcessService } from "./deletion-process.service";

describe("DeletionProcessService", () => {
    let service: DeletionProcessService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(DeletionProcessService);
    });

    it("should be created", () => {
        expect(service).toBeTruthy();
    });
});
