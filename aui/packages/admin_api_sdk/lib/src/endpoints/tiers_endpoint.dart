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
}
