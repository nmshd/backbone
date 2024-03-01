import { TestBed } from "@angular/core/testing";

import { QuotasService } from "./quotas.service";

describe("QuotasService", function () {
    let service: QuotasService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(QuotasService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
