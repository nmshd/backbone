import { TestBed } from "@angular/core/testing";

import { TierService } from "./tier.service";

describe("TierService", function () {
    let service: TierService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(TierService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
