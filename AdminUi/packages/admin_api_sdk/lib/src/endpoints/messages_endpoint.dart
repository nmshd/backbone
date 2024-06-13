import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class MessagesEndpoint extends Endpoint {
  MessagesEndpoint(super.dio);

  Future<ApiResponse<List<MessageOverview>>> getMessagesByParticipantAddress({
    required String address,
    required String type,
    required int pageNumber,
    required int pageSize,
  }) =>
      get(
        '/api/v1/Messages',
        query: {
          'participant': address,
          'type': type,
          'PageNumber': pageNumber,
          'PageSize': pageSize,
        },
        transformer: (e) => (e as List).map(MessageOverview.fromJson).toList(),
      );
}
