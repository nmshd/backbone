import { TestBed } from "@angular/core/testing";

import { SidebarService } from "./sidebar.service";

describe("SidebarService", function () {
    let service: SidebarService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(SidebarService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
