import { TestBed } from "@angular/core/testing";

import { ApiKeyInterceptor } from "./api-key.interceptor";

describe("ApiKeyInterceptor", function() {
    beforeEach(function() { return TestBed.configureTestingModule({
            providers: [ApiKeyInterceptor]
        }); }
    );

    it("should be created", function() {
        const interceptor: ApiKeyInterceptor = TestBed.inject(ApiKeyInterceptor);
        expect(interceptor).toBeTruthy();
    });
});
