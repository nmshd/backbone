import { TestBed } from "@angular/core/testing";

import { MessageService } from "./message.service";

describe("MessageService", function () {
    let service: MessageService;

    beforeEach(function () {
        TestBed.configureTestingModule({});
        service = TestBed.inject(MessageService);
    });

    it("should be created", async function () {
        await expect(service).toBeTruthy();
    });
});
