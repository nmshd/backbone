import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class QuotasService {
  apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = environment.apiUrl;
  }

  getMetrics(): Observable<HttpResponseEnvelope<Metric>> {
    return this.http.get<HttpResponseEnvelope<Metric>>(
      this.apiUrl + '/Metrics'
    );
  }

  getPeriods(): string[] {
    return ['Hour', 'Day', 'Week', 'Month', 'Year'];
  }

  createTierQuota(
    quota: TierQuota,
    tierId: string
  ): Observable<HttpResponseEnvelope<TierQuota>> {
    return this.http.post<HttpResponseEnvelope<TierQuota>>(
      this.apiUrl + '/Tiers/' + tierId + '/Quotas',
      quota
    );
  }
}

export interface Metric {
  id: string;
  key: string;
  displayName: string;
}

export interface TierQuota {
  id: string;
  metric: Metric;
  max: number;
  period: string;
}

export interface Quota {
  tierId?: string;
  metricKey: string;
  max: number;
  period: string;
}
