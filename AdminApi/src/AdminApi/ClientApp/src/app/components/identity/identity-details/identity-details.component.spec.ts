import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { BehaviorSubject } from "rxjs";
import { Identity } from "../../../../services/identity-service/identity.service";
import { TierOverview, TierService } from "../../../../services/tier-service/tier.service";
import { IdentityDetailsComponent, TierUtils } from "./identity-details.component";

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

describe("IdentityDetailsComponent", function () {
    let tierService: TierService;
    let mockRouter: Router;
    let identityDetailsComponent: IdentityDetailsComponent;
    let mockActivatedRoute: MockActivatedRoute;
    let httpTestingController: HttpTestingController;

    beforeEach(function () {
        mockRouter = {
            events: new BehaviorSubject(new NavigationEnd(0, "", ""))
        } as any;

        mockActivatedRoute = new MockActivatedRoute();

        TestBed.configureTestingModule({
            declarations: [IdentityDetailsComponent],
            imports: [HttpClientTestingModule],
            providers: [TierService, { provide: Router, useValue: mockRouter }, { provide: ActivatedRoute, useValue: mockActivatedRoute }]
        });

        tierService = TestBed.inject(TierService);
        httpTestingController = TestBed.inject(HttpTestingController);
    });

    afterEach(function () {
        httpTestingController.verify();
    });

    it("tier service should be created", async function () {
        await expect(tierService).toBeTruthy();
    });

    it("should be created", async function () {
        const fixture = TestBed.createComponent(IdentityDetailsComponent);
        identityDetailsComponent = fixture.componentInstance;
        await expect(identityDetailsComponent).toBeTruthy();
    });

    describe("TierUtils", function () {
        const tier1: TierOverview = {
            id: "1",
            name: "Tier 1",
            numberOfIdentities: 0,
            canBeUsedAsDefaultForUser: true,
            canBeManuallyAssigned: true
        };

        const tier2: TierOverview = {
            id: "2",
            name: "Tier 2",
            numberOfIdentities: 0,
            canBeUsedAsDefaultForUser: false,
            canBeManuallyAssigned: false
        };

        const tiers: TierOverview[] = [tier1, tier2];

        it("should return false if identity is not in a tier that cannot be unassigned", async function () {
            const identity: Identity = {
                address: "id1",
                clientId: "test",
                tierId: "1",
                devices: [],
                quotas: [],
                identityVersion: "1",
                createdAt: new Date(),
                publicKey: ""
            };

            await expect(TierUtils.isTierDisabled(tier1, tiers, identity)).toBe(false);
        });

        it("should return true if identity is in a tier that cannot be unassigned", async function () {
            const identity: Identity = {
                address: "id1",
                clientId: "test",
                tierId: "2",
                devices: [],
                quotas: [],
                identityVersion: "1",
                createdAt: new Date(),
                publicKey: ""
            };

            await expect(TierUtils.isTierDisabled(tier1, tiers, identity)).toBe(true);
        });

        it("should return false if tier cannot be manually assigned and is not the identity's tier", async function () {
            const identity: Identity = {
                address: "id1",
                clientId: "test",
                tierId: "1",
                devices: [],
                quotas: [],
                identityVersion: "1",
                createdAt: new Date(),
                publicKey: ""
            };

            await expect(TierUtils.isTierDisabled(tier2, tiers, identity)).toBe(false);
        });
    });
});
