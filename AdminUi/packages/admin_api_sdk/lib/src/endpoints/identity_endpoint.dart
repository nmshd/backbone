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

  // Future<ApiResponse<List<Identity>>> getIdentities({
  //   required IdentityOverviewFilter filter,
  //   required int pageNumber,
  //   required int pageSize,
  // }) {
  //   final paginationFilter = '\$top=$pageSize&\$skip=${pageNumber * pageSize}&\$count=true';
  //   final odataFilter = buildODataFilter(filter, paginationFilter);
  //   const odataExpand = r'&$expand=Tier';
  //   const odataUrl = '/odata/Identities';

  //   return get('$odataUrl$odataFilter$odataExpand', transformer: (dynamic response) {
  //     final result = (response['value'] as List<dynamic>).map(Identity.fromJson).toList();
  //     return result;
  //   });
  // }

  // String buildODataFilter(IdentityOverviewFilter filter, String paginationFilter) {
  //   final odataFilter = ODataFilterBuilder();

  //   if (filter.address != null && filter.address!.isNotEmpty) {
  //     odataFilter.contains("address", filter.address!);
  //   }

  //   if (filter.tiers != null && filter.tiers!.isNotEmpty) {
  //     final tiersFilter = ODataFilterBuilder();
  //     filter.tiers!.forEach((tier) {
  //       tiersFilter.or((x) => x.eq('tier/Id', tier));
  //     });
  //     odataFilter.and((_) => tiersFilter);
  //   }

  //   if (filter.clients != null && filter.clients!.length > 0) {
  //     const clientsFilter = new ODataFilterBuilder();
  //     filter.clients!.forEach((client) {
  //       clientsFilter.or((x) => x.eq('createdWithClient', client));
  //       ;
  //     });
  //     odataFilter.and((_) => clientsFilter);
  //   }

  //   if (filter.createdAt.operator != null && filter.createdAt.value != null) {
  //           switch (filter.createdAt.operator) {
  //               case ">":
  //                   odataFilter.gt('createdAt', filter.createdAt.value!.toIso8601String().substring(0,10), false);
  //                   break;
  //               case "<":
  //                   odataFilter.lt('createdAt', filter.createdAt.value!.toIso8601String().substring(0,10), false);
  //                   break;
  //               case "=":
  //                   odataFilter.eq('createdAt', filter.createdAt.value!.toIso8601String().substring(0,10), false);
  //                   break;
  //               case "<=":
  //                   odataFilter.le('createdAt', filter.createdAt.value!.toIso8601String().substring(0,10), false);
  //                   break;
  //               case ">=":
  //                   odataFilter.ge('createdAt', filter.createdAt.value!.toIso8601String().substring(0,10), false);
  //                   break;
  //               default:
  //                   this.logger.error(`Invalid createdAt filter operator: ${filter.createdAt.operator}`);
  //                   break;
  //           }
  //       }

  //     if (filter.lastLoginAt.operator != null && filter.lastLoginAt.value != null) {
  //           switch (filter.lastLoginAt.operator) {
  //               case ">":
  //                   odataFilter.gt("lastLoginAt", filter.lastLoginAt.value!.toIso8601String().substring(0, 10), false);
  //                   break;
  //               case "<":
  //                   odataFilter.lt("lastLoginAt", filter.lastLoginAt.value!.toIso8601String().substring(0, 10), false);
  //                   break;
  //               case "=":
  //                   odataFilter.eq("lastLoginAt", filter.lastLoginAt.value!.toIso8601String().substring(0, 10), false);
  //                   break;
  //               case "<=":
  //                   odataFilter.le("lastLoginAt", filter.lastLoginAt.value!.toIso8601String().substring(0, 10), false);
  //                   break;
  //               case ">=":
  //                   odataFilter.ge("lastLoginAt", filter.lastLoginAt.value!.toIso8601String().substring(0, 10), false);
  //                   break;
  //               default:
  //                   this.logger.error(`Invalid lastLoginAt filter operator: ${filter.lastLoginAt.operator}`);
  //                   break;
  //           }
  //       }

  //       if (filter.numberOfDevices.operator != null && filter.numberOfDevices.value != null) {
  //           switch (filter.numberOfDevices.operator) {
  //               case ">":
  //                   odataFilter.gt("numberOfDevices", filter.numberOfDevices.value);
  //                   break;
  //               case "<":
  //                   odataFilter.lt("numberOfDevices", filter.numberOfDevices.value);
  //                   break;
  //               case "=":
  //                   odataFilter.eq("numberOfDevices", filter.numberOfDevices.value);
  //                   break;
  //               case "<=":
  //                   odataFilter.le("numberOfDevices", filter.numberOfDevices.value);
  //                   break;
  //               case ">=":
  //                   odataFilter.ge("numberOfDevices", filter.numberOfDevices.value);
  //                   break;
  //               default:
  //                   this.logger.error(`Invalid numberOfDevices filter operator: ${filter.numberOfDevices.operator}`);
  //                   break;
  //           }
  //       }

  //       if (filter.datawalletVersion.operator != null && filter.datawalletVersion.value != null) {
  //           switch (filter.datawalletVersion.operator) {
  //               case ">":
  //                   odataFilter.gt("datawalletVersion", filter.datawalletVersion.value);
  //                   break;
  //               case "<":
  //                   odataFilter.lt("datawalletVersion", filter.datawalletVersion.value);
  //                   break;
  //               case "=":
  //                   odataFilter.eq("datawalletVersion", filter.datawalletVersion.value);
  //                   break;
  //               case "<=":
  //                   odataFilter.le("datawalletVersion", filter.datawalletVersion.value);
  //                   break;
  //               case ">=":
  //                   odataFilter.ge("datawalletVersion", filter.datawalletVersion.value);
  //                   break;
  //               default:
  //                   this.logger.error(`Invalid datawalletVersion filter operator: ${filter.datawalletVersion.operator}`);
  //                   break;
  //           }
  //       }

  //       if (filter.identityVersion.operator != null && filter.identityVersion.value != null) {
  //           switch (filter.identityVersion.operator) {
  //               case ">":
  //                   odataFilter.gt("identityVersion", filter.identityVersion.value);
  //                   break;
  //               case "<":
  //                   odataFilter.lt("identityVersion", filter.identityVersion.value);
  //                   break;
  //               case "=":
  //                   odataFilter.eq("identityVersion", filter.identityVersion.value);
  //                   break;
  //               case "<=":
  //                   odataFilter.le("identityVersion", filter.identityVersion.value);
  //                   break;
  //               case ">=":
  //                   odataFilter.ge("identityVersion", filter.identityVersion.value);
  //                   break;
  //               default:
  //                   this.logger.error(`Invalid identityVersion filter operator: ${filter.identityVersion.operator}`);
  //                   break;
  //           }
  //       }

  //   final filterComponents = [if (odataFilter.toString() != '') '\$filter=$odataFilter', paginationFilter];
  //   final filterParameter = '?${filterComponents.join('&')}';

  //   return filterParameter;
  // }
}
