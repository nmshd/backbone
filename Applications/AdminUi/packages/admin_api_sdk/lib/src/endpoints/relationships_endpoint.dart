import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class RelationshipsEndpoint extends Endpoint {
  RelationshipsEndpoint(super._dio);

  Future<ApiResponse<List<Relationship>>> getRelationshipsByParticipantAddress(String participant, {int? pageNumber, int? pageSize}) {
    assert(pageNumber == null || pageNumber > 0, 'pageNumber must be greater than 0 when defined');

    return get(
      '/api/v2/Relationships',
      query: {'participant': participant, 'PageNumber': pageNumber, 'PageSize': pageSize},
      transformer: (e) => (e as List).map(Relationship.fromJson).toList(),
    );
  }
}
