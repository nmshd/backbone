import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class RelationshipsEndpoint extends Endpoint {
  RelationshipsEndpoint(super._dio);

  Future<ApiResponse<List<Relationship>>> getRelationshipsByParticipantAddress(
    String participant,
    int pageNumber,
    int pageSize,
  ) =>
      get(
        '/api/v1/Relationships',
        query: {
          'participant': participant,
          'pageNumber': pageNumber + 1,
          'pageSize': pageSize,
        },
        transformer: (e) => (e as List).map(Relationship.fromJson).toList(),
      );
}
