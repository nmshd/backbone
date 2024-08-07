import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class QuotasEndpoint extends Endpoint {
  QuotasEndpoint(super.dio);

  Future<ApiResponse<TierQuotaDefinition>> createTierQuota({
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
        transformer: TierQuotaDefinition.fromJson,
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
}
