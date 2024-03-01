import { Component } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { filter } from "rxjs/operators";
import { Breadcrumb, BreadcrumbService } from "src/app/services/breadcrumb-service/breadcrumb.service";

@Component({
    selector: "app-breadcrumb",
    templateUrl: "./breadcrumb.component.html",
    styleUrls: ["./breadcrumb.component.css"]
})
export class BreadcrumbComponent {
    public breadcrumbHistory: Breadcrumb[] = [];

    public constructor(
        private readonly breadcrumbService: BreadcrumbService,
        private readonly router: Router
    ) {}

    public ngOnInit(): void {
        this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
            this.breadcrumbHistory = this.breadcrumbService.getBreadcrumbHistory();
        });
    }

    public onBreadcrumbClick(index: number): void {
        this.breadcrumbService.clearBreadcrumbHistoryAfterIndex(index);
        this.breadcrumbHistory = this.breadcrumbService.getBreadcrumbHistory();
    }
}
