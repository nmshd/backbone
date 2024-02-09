import { TestBed } from "@angular/core/testing";
import { BreadcrumbService } from "./breadcrumb.service";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { BehaviorSubject } from "rxjs";

class MockActivatedRoute {
    private readonly subject = new BehaviorSubject(this.testParams);
    public params = this.subject.asObservable();

    private _testParams = {};
    public get testParams() {
        return this._testParams;
    }
    public set testParams(params: {}) {
        this._testParams = params;
        this.subject.next(params);
    }
}

describe("BreadcrumbService", function () {
    let breadcrumbService: BreadcrumbService;
    let mockRouter: Router;
    let mockActivatedRoute: MockActivatedRoute;

    const initialHistory = [
        { label: "Home", url: "/home" },
        { label: "Details", url: "/details" },
        { label: "Products", url: "/products" }
    ];

    beforeEach(function () {
        mockRouter = {
            events: new BehaviorSubject(new NavigationEnd(0, "", ""))
        } as any;

        mockActivatedRoute = new MockActivatedRoute();

        TestBed.configureTestingModule({
            providers: [BreadcrumbService, { provide: Router, useValue: mockRouter }, { provide: ActivatedRoute, useValue: mockActivatedRoute }]
        });
        breadcrumbService = TestBed.inject(BreadcrumbService);
    });

    it("should be created", async function () {
        await expect(breadcrumbService).toBeTruthy();
    });

    it("should return an empty array initially", async function () {
        // Arrange

        // Act
        const result = breadcrumbService.getBreadcrumbHistory();

        // Assert
        await expect(result.length).toEqual(0);
    });

    it("should return the breadcrumb history array", async function () {
        // Arrange
        const expectedHistorySteps = [
            { label: "Home", url: "/home" },
            { label: "Details", url: "/details" },
            { label: "Products", url: "/products" }
        ];

        // Act
        for (const historyStep of expectedHistorySteps) {
            breadcrumbService["breadcrumbHistory"].push(historyStep);
        }
        const result = breadcrumbService.getBreadcrumbHistory();

        // Assert
        await expect(result).toEqual(expectedHistorySteps);
    });

    it("should return a flat array when multiple trails are added", async function () {
        // Arrange
        const firstTrail = [{ label: "Home", url: "/home" }];
        const secondTrail = [
            { label: "Details", url: "/details" },
            { label: "Products", url: "/products" }
        ];

        breadcrumbService["breadcrumbHistory"] = [...firstTrail, ...secondTrail];

        // Act
        const result = breadcrumbService.getBreadcrumbHistory();

        // Assert
        await expect(result).toEqual([...firstTrail, ...secondTrail]);
    });

    it("should return a copy of the breadcrumb history array", async function () {
        // Arrange
        const expectedHistorySteps = [
            { label: "Home", url: "/home" },
            { label: "Details", url: "/details" },
            { label: "Products", url: "/products" }
        ];

        // Act
        for (const historyStep of expectedHistorySteps) {
            breadcrumbService["breadcrumbHistory"].push(historyStep);
        }
        const result = JSON.parse(JSON.stringify(breadcrumbService.getBreadcrumbHistory()));
        result[0].label = "Modified";

        // Assert
        await expect(result).not.toEqual(expectedHistorySteps);
    });

    it("should not modify the internal breadcrumb history array when modifying the result", async function () {
        // Arrange
        const expectedHistorySteps = [
            { label: "Home", url: "/home" },
            { label: "Details", url: "/details" },
            { label: "Products", url: "/products" }
        ];

        // Act
        for (const historyStep of expectedHistorySteps) {
            breadcrumbService["breadcrumbHistory"].push(historyStep);
        }
        const result = breadcrumbService.getBreadcrumbHistory();

        // Modify the result
        result[0].label = "Modified";

        // Assert
        await expect(breadcrumbService.getBreadcrumbHistory()).toEqual(expectedHistorySteps);
    });

    it("should do nothing if the index is negative", async function () {
        // Arrange
        breadcrumbService["breadcrumbHistory"] = [...initialHistory];

        // Act
        breadcrumbService.clearBreadcrumbHistoryAfterIndex(-1);

        // Assert
        await expect(breadcrumbService.getBreadcrumbHistory()).toEqual(initialHistory);
    });

    it("should do nothing if the index is equal to the last index", async function () {
        // Arrange
        breadcrumbService["breadcrumbHistory"] = [...initialHistory];
        const lastIndex = initialHistory.length - 1;

        // Act
        breadcrumbService.clearBreadcrumbHistoryAfterIndex(lastIndex);

        // Assert
        await expect(breadcrumbService.getBreadcrumbHistory()).toEqual(initialHistory);
    });

    it("should clear history after the specified index", async function () {
        // Arrange
        const expectedHistorySteps = [
            { label: "Home", url: "/home" },
            { label: "Details", url: "/details" }
        ];
        breadcrumbService["breadcrumbHistory"] = [...initialHistory];
        const indexToClear = 1;

        // Act
        breadcrumbService.clearBreadcrumbHistoryAfterIndex(indexToClear);

        // Assert
        await expect(breadcrumbService.getBreadcrumbHistory()).toEqual(expectedHistorySteps);
    });
});
