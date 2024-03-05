import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class ClientsEndpoint extends Endpoint {
  ClientsEndpoint(super.dio);

  Future<ApiResponse<List<Clients>>> getClients() => get(
        '/api/v1/Clients',
        transformer: (e) => (e as List).map(Clients.fromJson).toList(),
      );

  Future<ApiResponse<Client>> createClient({
    required String defaultTier,
    String? clientId,
    String? clientSecret,
    String? displayName,
    int? maxIdentities,
  }) =>
      post(
        '/api/v1/Clients',
        data: {
          'defaultTier': defaultTier,
          'clientId': clientId,
          'clientSecret': clientSecret,
          'displayName': displayName,
          'maxIdentities': maxIdentities,
        },
        transformer: Client.fromJson,
      );

  Future<ApiResponse<Client>> getClient(String clientId) => get(
        '/api/v1/Clients/$clientId',
        transformer: Client.fromJson,
      );

  Future<ApiResponse<Client>> changeClientSecret(String clientId, {required String? newSecret}) => patch(
        '/api/v1/Clients/$clientId/ChangeSecret',
        data: {
          'newSecret': newSecret,
        },
        transformer: Client.fromJson,
      );

  Future<ApiResponse<Client>> updateClient(
    String clientId, {
    required String defaultTier,
    required int? maxIdentities,
  }) =>
      put(
        '/api/v1/Clients/$clientId',
        data: {
          'defaultTier': defaultTier,
          'maxIdentities': maxIdentities,
        },
        transformer: Client.fromJson,
      );

  Future<ApiResponse<void>> deleteClient(String clientId) => delete(
        '/api/v1/Clients/$clientId',
        expectedStatus: 204,
        transformer: (e) {},
        allowEmptyResponse: true,
      );
}
