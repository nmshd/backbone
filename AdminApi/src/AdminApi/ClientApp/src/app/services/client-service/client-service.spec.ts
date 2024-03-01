import { TestBed } from "@angular/core/testing";

import { ClientService } from "./client-service";

describe("ClientServiceService", function () {
    let service: ClientService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(ClientService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
