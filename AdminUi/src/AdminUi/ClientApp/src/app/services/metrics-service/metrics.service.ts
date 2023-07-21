import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpResponseEnvelope } from "src/app/utils/http-response-envelope";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class MetricsService {
    apiUrl: string;

    constructor(private http: HttpClient) {
        this.apiUrl = environment.apiUrl;
    }

    getMetrics(): Observable<HttpResponseEnvelope<Metric>> {
        return this.http.get<HttpResponseEnvelope<Metric>>(this.apiUrl + "/Metrics");
    }
}

export interface Metric {
    key: string;
    displayName: string;
}
