import 'package:admin_api_types/admin_api_types.dart';

import '../builders/builders.dart';
import '../types/types.dart';
import 'endpoint.dart';

class IdentitiesEndpoint extends Endpoint {
  IdentitiesEndpoint(super._dio);

  Future<ApiResponse<Identity>> getIdentity(String address) => get('/api/v2/Identities/$address', transformer: Identity.fromJson);

  Future<ApiResponse<void>> updateIdentity(String address, {required String tierId}) =>
      put('/api/v2/Identities/$address', data: {'tierId': tierId}, transformer: (e) {});

  Future<ApiResponse<List<IdentityOverview>>> getIdentities({
    String orderBy = 'address asc',
    IdentityOverviewFilter? filter,
    int pageNumber = 0,
    int pageSize = 10,
  }) {
    final queryParameters = <String, String>{r'$expand': 'Tier'};

    if (filter != null) {
      queryParameters[r'$filter'] = IdentityOverviewFilterBuilder(filter).build();
    }

    if (queryParameters[r'$filter'] == '') {
      queryParameters.remove(r'$filter');
    }

    return getOData(
      '/odata/Identities',
      query: queryParameters,
      orderBy: orderBy,
      transformer: (e) => (e as List).map(IdentityOverview.fromJson).toList(),
      pageNumber: pageNumber,
      pageSize: pageSize,
    );
  }

  Future<ApiResponse<List<IdentityDeletionProcess>>> getIdentityDeletionProcesses({required String address}) =>
      get('/api/v2/Identities/$address/DeletionProcesses', transformer: (e) => (e as List).map(IdentityDeletionProcess.fromJson).toList());

  Future<ApiResponse<IdentityDeletionProcessDetail>> getIdentityDeletionProcess({required String address, required String deletionProcessId}) =>
      get('/api/v2/Identities/$address/DeletionProcesses/$deletionProcessId', transformer: IdentityDeletionProcessDetail.fromJson);

  Future<ApiResponse<List<IdentityDeletionProcessAuditLogEntry>>> getIdentityDeletionProcessAuditLogs({required String address}) => get(
    '/api/v2/Identities/$address/DeletionProcesses/AuditLogs',
    transformer: (e) => (e as List).map(IdentityDeletionProcessAuditLogEntry.fromJson).toList(),
  );

  Future<ApiResponse<IndividualQuota>> createIndividualQuota({
    required String address,
    required String metricKey,
    required int max,
    required String period,
  }) =>
      post('/api/v2/Identities/$address/Quotas', data: {'metricKey': metricKey, 'max': max, 'period': period}, transformer: IndividualQuota.fromJson);

  Future<ApiResponse<void>> deleteIndividualQuota({required String address, required String individualQuotaId}) =>
      delete('/api/v2/Identities/$address/Quotas/$individualQuotaId', expectedStatus: 204, transformer: (e) {}, allowEmptyResponse: true);
}
