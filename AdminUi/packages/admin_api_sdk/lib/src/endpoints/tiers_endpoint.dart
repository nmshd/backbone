import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class TiersEndpoint extends Endpoint {
  TiersEndpoint(super.dio);

  Future<ApiResponse<List<Tier>>> getTiers() => get(
        '/api/v1/Tiers',
        transformer: (e) => (e as List).map(Tier.fromJson).toList(),
      );

  Future<ApiResponse<Tier>> createTier({
    required String name,
  }) =>
      post(
        '/api/v1/Tiers',
        data: {
          'name': name,
        },
        transformer: Tier.fromJson,
      );

  Future<ApiResponse<TierOverview>> getTier(
    String tierId,
  ) =>
      get(
        '/api/v1/Tiers/$tierId',
        transformer: TierOverview.fromJson,
      );

  Future<ApiResponse<void>> deleteTier(String tierId) => delete(
        '/api/v1/Tiers/$tierId',
        expectedStatus: 204,
        transformer: (e) {},
        allowEmptyResponse: true,
      );
}
