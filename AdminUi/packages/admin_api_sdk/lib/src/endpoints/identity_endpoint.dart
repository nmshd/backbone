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

  Future<ApiResponse<ODataResponse>> getIdentities() => get(
        '/odata/Identities',
        query: {
          r'$count': 'true',
          r'$expand': 'Tier',
        },
        transformer: ODataResponse.fromJson,
      );
}
