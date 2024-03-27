import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class DeletionProcessesEndpoint extends Endpoint {
  DeletionProcessesEndpoint(super._dio);

  Future<ApiResponse<List<DeletionProcessOverview>>> getDeletionProcesses({
    required String address,
  }) =>
      get(
        '/api/v1/Identities/$address/DeletionProcesses',
        transformer: (e) => (e as List).map(DeletionProcessOverview.fromJson).toList(),
      );

  Future<ApiResponse<DeletionProcessDetails>> getDeletionProcessDetails({
    required String address,
    required String deletionProcessID,
  }) =>
      get(
        '/api/v1/Identities/$address/DeletionProcesses/$deletionProcessID',
        transformer: DeletionProcessDetails.fromJson,
      );

  Future<ApiResponse<void>> updateDeletionProcessDetails({
    required String address,
    required String deletionProcessID,
  }) =>
      put(
        '/api/v1/Identities/$address/DeletionProcesses/$deletionProcessID/Cancel',
        transformer: (e) {},
      );
}
