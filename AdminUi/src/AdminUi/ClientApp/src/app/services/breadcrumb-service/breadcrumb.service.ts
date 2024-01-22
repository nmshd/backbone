import { Injectable } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { filter } from "rxjs/operators";

@Injectable({
    providedIn: "root"
})
export class BreadcrumbService {
    private breadcrumbHistory: Breadcrumb[] = [];
    private readonly maxHistorySize = 10;

    public constructor(
        private readonly router: Router,
        private readonly activatedRoute: ActivatedRoute
    ) {
        const storedHistory = localStorage.getItem("breadcrumb-history");
        if (storedHistory) {
            this.breadcrumbHistory = JSON.parse(storedHistory);
        }
        this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
            this.updateBreadcrumbHistory();
            localStorage.setItem("breadcrumb-history", JSON.stringify(this.breadcrumbHistory));
        });
    }

    public getBreadcrumbHistory(): Breadcrumb[] {
        return [...this.breadcrumbHistory];
    }

    public clearBreadcrumbHistoryAfterIndex(index: number): void {
        if (index >= 0 && index < this.breadcrumbHistory.length - 1) {
            this.breadcrumbHistory.splice(index + 1);
        }
    }

    private updateBreadcrumbHistory(): void {
        const breadcrumbs: Breadcrumb[] = [];
        this.generateBreadcrumbs(this.activatedRoute.root, "", breadcrumbs);

        const breadcrumbTrail = [...breadcrumbs];

        if (this.shouldClearBreadcrumbHistory(breadcrumbTrail)) {
            this.breadcrumbHistory = [];
            return;
        }

        if (this.isMainLinkClicked(breadcrumbTrail)) {
            this.breadcrumbHistory = breadcrumbTrail;
        } else {
            if (this.shouldPushBreadcrumbTrail(breadcrumbTrail)) {
                this.breadcrumbHistory.push(...breadcrumbTrail);
            }

            if (this.breadcrumbHistory.length > this.maxHistorySize) {
                this.trimBreadcrumbHistory();
            }
        }
    }

    private isMainLinkClicked(breadcrumbTrail: Breadcrumb[]): boolean {
        return breadcrumbTrail.length === 1 && breadcrumbTrail[0].url.split("/").length === 2;
    }

    private shouldClearBreadcrumbHistory(trail: Breadcrumb[]): boolean {
        return trail.some((breadcrumb) => breadcrumb.url.includes("login"));
    }

    private shouldPushBreadcrumbTrail(breadcrumbTrail: Breadcrumb[]): boolean {
        return this.breadcrumbHistory.length === 0 || this.breadcrumbHistory[this.breadcrumbHistory.length - 1].url !== breadcrumbTrail[breadcrumbTrail.length - 1].url;
    }

    private trimBreadcrumbHistory(): void {
        const currentSize = this.breadcrumbHistory.length;
        if (currentSize > this.maxHistorySize) {
            this.breadcrumbHistory = this.breadcrumbHistory.slice(currentSize - this.maxHistorySize);
        }
    }

    private generateBreadcrumbs(route: ActivatedRoute | null, url = "", breadcrumbs: Breadcrumb[] = []): void {
        if (!route?.children) {
            return;
        }

        const children: ActivatedRoute[] = route.children;

        for (const child of children) {
            const routeURL: string = child.snapshot.url.map((segment) => segment.path).join("/");

            const breadcrumbLabel: string = child.snapshot.data.breadcrumb;

            if (routeURL !== "") {
                url += `/${routeURL}`;
                const dynamicData = this.extractDynamicData(routeURL);

                breadcrumbs.push({
                  label: dynamicData !== "" ? dynamicData : breadcrumbLabel,
                  url: url
                });
              }

            this.generateBreadcrumbs(child, url, breadcrumbs);
        }
    }

    private extractDynamicData(routeURL: string): string {
        const segments = routeURL.split("/");
        return segments.length === 2 ? segments[1] : "";
      }
}

export interface Breadcrumb {
    label: string;
    url: string;
}
