import { ActivatedRouteSnapshot, BaseRouteReuseStrategy } from "@angular/router";

export class CustomRouteReuseStrategy extends BaseRouteReuseStrategy {
    public override shouldReuseRoute(future: ActivatedRouteSnapshot, _curr: ActivatedRouteSnapshot): boolean {
        return future.data["reuseComponent"];
    }
}
