import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class DeletionProcessesEndpoint extends Endpoint {
  DeletionProcessesEndpoint(super._dio);

  Future<ApiResponse<List<DeletionProcess>>> getIdentityDeletionProcesses({
    required String address,
  }) =>
      get(
        '/api/v1/Identities/$address/DeletionProcesses',
        transformer: (e) => (e as List).map(DeletionProcess.fromJson).toList(),
      );

  Future<ApiResponse<DeletionProcessDetail>> getIdentityDeletionProcessDetails({
    required String address,
    required String deletionProcessId,
  }) =>
      get(
        '/api/v1/Identities/$address/DeletionProcesses/$deletionProcessId',
        transformer: DeletionProcessDetail.fromJson,
      );

  Future<ApiResponse<void>> cancelDeletionProcessAsSupport({
    required String address,
    required String deletionProcessId,
  }) =>
      put(
        '/api/v1/Identities/$address/DeletionProcesses/$deletionProcessId/Cancel',
        transformer: (e) {},
      );
}
