import { TestBed } from "@angular/core/testing";

import { MetricsService } from "./metrics.service";

describe("MetricsService", function() {
    let service: MetricsService;

    beforeEach(function() {
        TestBed.configureTestingModule({});
        service = TestBed.inject(MetricsService);
    });

    it("should be created", function() {
        expect(service).toBeTruthy();
    });
});
