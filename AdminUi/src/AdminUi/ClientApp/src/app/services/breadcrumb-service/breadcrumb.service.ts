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
        const storedHistory = sessionStorage.getItem("breadcrumb-history");
        if (storedHistory) {
            this.breadcrumbHistory = JSON.parse(storedHistory);
        }
        this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
            this.updateBreadcrumbHistory();
            sessionStorage.setItem("breadcrumb-history", JSON.stringify(this.breadcrumbHistory));
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
        const breadcrumbHistory = this.generateBreadcrumbHistory(this.activatedRoute.root, "");

        if (this.shouldClearBreadcrumbHistory(breadcrumbHistory)) {
            this.breadcrumbHistory = [];
            return;
        }

        if (this.isMainLinkClicked(breadcrumbHistory)) {
            this.breadcrumbHistory = breadcrumbHistory;
        } else {
            const existingRouteIndex = this.findExistingRouteIndex(breadcrumbHistory);
            if (existingRouteIndex !== -1) {
                this.clearBreadcrumbHistoryAfterIndex(existingRouteIndex);
            } else if (this.shouldPushBreadcrumbHistory(breadcrumbHistory)) {
                this.breadcrumbHistory.push(...breadcrumbHistory);
            }

            if (this.breadcrumbHistory.length > this.maxHistorySize) {
                this.trimBreadcrumbHistory();
            }
        }
    }

    private findExistingRouteIndex(newBreadcrumbHistory: Breadcrumb[]): number {
        for (const i in this.breadcrumbHistory) {
            const existingBreadcrumb = this.breadcrumbHistory[i];
            const newBreadcrumb = newBreadcrumbHistory.find((breadcrumb) => breadcrumb.url === existingBreadcrumb.url);
            if (newBreadcrumb) {
                return parseInt(i, 10);
            }
        }
        return -1;
    }

    private isMainLinkClicked(breadcrumbHistory: Breadcrumb[]): boolean {
        return breadcrumbHistory.length === 1 && breadcrumbHistory[0].url.split("/").length === 2;
    }

    private shouldClearBreadcrumbHistory(trail: Breadcrumb[]): boolean {
        return trail.some((breadcrumb) => breadcrumb.url.includes("login"));
    }

    private shouldPushBreadcrumbHistory(breadcrumbHistory: Breadcrumb[]): boolean {
        return this.breadcrumbHistory.length === 0 || this.breadcrumbHistory[this.breadcrumbHistory.length - 1].url !== breadcrumbHistory[breadcrumbHistory.length - 1].url;
    }

    private trimBreadcrumbHistory(): void {
        const currentSize = this.breadcrumbHistory.length;
        if (currentSize > this.maxHistorySize) {
            this.breadcrumbHistory = this.breadcrumbHistory.slice(currentSize - this.maxHistorySize);
        }
    }

    private generateBreadcrumbHistory(route: ActivatedRoute | null, url = ""): Breadcrumb[] {
        if (!route?.children) {
            return [];
        }
        const breadcrumbHistory: Breadcrumb[] = [];

        const children = route.children;

        for (const child of children) {
            const routeURL = child.snapshot.url.map((segment) => segment.path).join("/");

            const breadcrumbLabel = child.snapshot.data.breadcrumb;

            if (routeURL !== "") {
                url += `/${routeURL}`;
                const dynamicData = this.extractDynamicData(routeURL);

                breadcrumbHistory.push({
                    label: dynamicData !== "" ? dynamicData : breadcrumbLabel,
                    url: url
                });
            }

            breadcrumbHistory.push(...this.generateBreadcrumbHistory(child, url));
        }

        return breadcrumbHistory;
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
