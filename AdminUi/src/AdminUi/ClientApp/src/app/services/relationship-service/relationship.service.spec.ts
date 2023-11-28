import { TestBed } from "@angular/core/testing";

import { RelationshipService } from "./relationship.service";

describe("RelationshipService", function () {
    let service: RelationshipService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(RelationshipService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
