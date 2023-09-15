import { TestBed } from "@angular/core/testing";

import { AuthGuardGuard } from "./auth-guard.guard";

describe("AuthGuardGuard", function() {
    let guard: AuthGuardGuard;

    beforeEach(function() {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(AuthGuardGuard);
    });

    it("should be created", function() {
        expect(guard).toBeTruthy();
    });
});
