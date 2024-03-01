import { Component } from "@angular/core";

@Component({
    selector: "app-identity-list",
    templateUrl: "./identity-list.component.html",
    styleUrls: ["./identity-list.component.css"]
})
export class IdentityListComponent {
    public header: string;
    public headerDescription: string;

    public constructor() {
        this.header = "Identities";
        this.headerDescription = "A list of existing Identities";
    }
}
