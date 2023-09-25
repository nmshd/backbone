import { Component } from "@angular/core";

@Component({
    selector: "app-page-not-found",
    templateUrl: "./page-not-found.component.html",
    styleUrls: ["./page-not-found.component.css"]
})
export class PageNotFoundComponent {
    public error: ErrorInfo;

    public constructor() {
        this.error = {
            code: 0,
            title: "",
            description: ""
        };
    }

    public ngOnInit(): void {
        this.error = {
            code: 404,
            title: "Page not found",
            description: "Sorry, the page you were looking for does not exist or has been moved."
        };
    }
}

interface ErrorInfo {
    code: number;
    title: string;
    description: string;
}
