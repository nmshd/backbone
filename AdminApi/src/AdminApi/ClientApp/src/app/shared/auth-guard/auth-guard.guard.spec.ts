import { TestBed } from "@angular/core/testing";

import { AuthGuard } from "./auth-guard.guard";

describe("AuthGuardGuard", function () {
    let guard: AuthGuard;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(AuthGuard);
    });

    it("should be created", async function () {
        await expect(guard).toBeTruthy();
    });
});
