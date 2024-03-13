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

  Future<ApiResponse<List<IdentityOverview>>> getIdentities({
    required IdentityOverviewFilter filter,
    int pageNumber = 0,
    int pageSize = 10,
  }) =>
      getOData(
        '/odata/Identities',
        query: {
          r'$filter': buildFilter(filter: filter),
          r'$top': '$pageSize',
          r'$skip': '${pageNumber * pageSize}',
          r'$count': 'true',
          r'$expand': 'Tier',
        },
        transformer: (e) => (e as List).map(IdentityOverview.fromJson).toList(),
      );

  String buildFilter({IdentityOverviewFilter? filter}) {
    final filterBuilder = ODataFilterBuilder();

    if (filter?.address != null) {
      filterBuilder.addFilter("contains(address, '${filter!.address}')");
    }

    if (filter?.tiers != null) {
      final tiersFilter = filter!.tiers!.map((tier) => "tier/Id eq '$tier'").join(' or ');
      filterBuilder.addFilter('($tiersFilter)');
    }

    if (filter?.clients != null) {
      final clientsFilter = filter!.clients!.map((client) => "createdWithClient eq '$client'").join(' or ');
      filterBuilder.addFilter('($clientsFilter)');
    }

    if (filter?.createdAt != null) {
      filterBuilder.addFilter('createdAt ${getOperatorString(filter!.createdAt!.operator)} ${filter.createdAt!.value.substring(0, 10)}');
    }

    if (filter?.lastLoginAt != null) {
      filterBuilder.addFilter('lastLoginAt ${getOperatorString(filter!.lastLoginAt!.operator)} ${filter.lastLoginAt!.value.substring(0, 10)}');
    }

    if (filter?.numberOfDevices != null) {
      filterBuilder.addFilter('numberOfDevices ${getOperatorString(filter!.numberOfDevices!.operator)} ${filter.numberOfDevices!.value}');
    }

    if (filter?.datawalletVersion != null) {
      filterBuilder.addFilter('datawalletVersion ${getOperatorString(filter!.datawalletVersion!.operator)} ${filter.datawalletVersion!.value}');
    }

    if (filter?.identityVersion != null) {
      filterBuilder.addFilter('identityVersion ${getOperatorString(filter!.identityVersion!.operator)} ${filter.identityVersion!.value}');
    }

    return filterBuilder.buildFilter();
  }
}

class ODataFilterBuilder {
  String _filter = '';

  void addFilter(String filter) {
    if (_filter.isNotEmpty) {
      _filter += ' and ';
    }
    _filter += '($filter)';
  }

  String buildFilter() {
    return _filter.isNotEmpty ? _filter : '';
  }
}

String getOperatorString(FilterOperator operator) {
  switch (operator) {
    case FilterOperator.equal:
      return 'eq';
    case FilterOperator.notEqual:
      return 'ne';
    case FilterOperator.greaterThan:
      return 'gt';
    case FilterOperator.greaterThanOrEqual:
      return 'ge';
    case FilterOperator.lessThan:
      return 'lt';
    case FilterOperator.lessThanOrEqual:
      return 'le';
  }
}
