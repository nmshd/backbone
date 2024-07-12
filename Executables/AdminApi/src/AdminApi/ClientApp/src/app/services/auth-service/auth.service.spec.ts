import { TestBed } from "@angular/core/testing";

import { AuthService } from "./auth.service";

describe("AuthService", function () {
    let service: AuthService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(AuthService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
