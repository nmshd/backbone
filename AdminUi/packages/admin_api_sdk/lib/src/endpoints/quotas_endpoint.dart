import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class QuotasEndpoint extends Endpoint {
  QuotasEndpoint(super.dio);

  Future<ApiResponse<Quota>> createTierQuota({
    required String tierId,
    required String metricKey,
    required int max,
    required String period,
  }) =>
      post(
        '/api/v1/Tiers/$tierId/Quotas',
        data: {
          'metricKey': metricKey,
          'max': max,
          'period': period,
        },
        transformer: Quota.fromJson,
      );

  Future<ApiResponse<void>> deleteTierQuota({
    required String tierId,
    required String tierQuotaDefinitionId,
  }) =>
      delete(
        '/api/v1/Tiers/$tierId/Quotas/$tierQuotaDefinitionId',
        expectedStatus: 204,
        transformer: (e) {},
        allowEmptyResponse: true,
      );

  Future<ApiResponse<Quota>> createIdentityQuota({
    required String identityId,
    required String metricKey,
    required int max,
    required String period,
  }) =>
      post(
        '/api/v1/Identities/$identityId/Quotas',
        data: {
          'metricKey': metricKey,
          'max': max,
          'period': period,
        },
        transformer: Quota.fromJson,
      );

  Future<ApiResponse<void>> deleteIdentityQuota({
    required String tierId,
    required String individualQuotaId,
  }) =>
      delete(
        '/api/v1/Tiers/$tierId/Quotas/$individualQuotaId',
        expectedStatus: 204,
        transformer: (e) {},
        allowEmptyResponse: true,
      );

  Future<ApiResponse<List<Metric>>> getMetrics() => get(
        '/api/v1/Metrics',
        transformer: (e) => (e as List).map(Metric.fromJson).toList(),
      );
}
