import { TestBed } from "@angular/core/testing";

import { ClientServiceService } from "./client-service";

describe("ClientServiceService", function() {
    let service: ClientServiceService;

    beforeEach(function() {
        TestBed.configureTestingModule({});
        service = TestBed.inject(ClientServiceService);
    });

    it("should be created", function() {
        expect(service).toBeTruthy();
    });
});
