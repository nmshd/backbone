import { TestBed } from "@angular/core/testing";

import { IdentityService } from "./identity.service";

describe("IdentityServiceService", function () {
    let service: IdentityService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(IdentityService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
