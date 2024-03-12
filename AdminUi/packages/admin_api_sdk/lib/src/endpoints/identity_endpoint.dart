import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class IdentitiesEndpoint extends Endpoint {
  IdentitiesEndpoint(super._dio);

  Future<ApiResponse<Identity>> getIdentity(
    String address,
  ) =>
      get(
        '/api/v1/Identities/$address',
        transformer: Identity.fromJson,
      );

  Future<ApiResponse<void>> updateIdentity(
    String address, {
    required String tierId,
  }) =>
      put(
        '/api/v1/Identities/$address',
        data: {
          'tierId': tierId,
        },
        transformer: (e) {},
      );

  Future<ApiResponse<List<IdentityOverview>>> getIdentities() => getOData(
        '/odata/Identities',
        query: {
          r'$count': 'true',
          r'$expand': 'Tier',
        },
        transformer: (e) => (e as List).map(IdentityOverview.fromJson).toList(),
      );
}
