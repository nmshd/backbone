import { TestBed } from "@angular/core/testing";

import { ApiKeyInterceptor } from "./api-key.interceptor";

describe("ApiKeyInterceptor", function () {
    beforeEach(function () {
        return TestBed.configureTestingModule({
            providers: [ApiKeyInterceptor]
        });
    });

    it("should be created", async function () {
        const interceptor: ApiKeyInterceptor = TestBed.inject(ApiKeyInterceptor);
        await expect(interceptor).toBeTruthy();
    });
});
