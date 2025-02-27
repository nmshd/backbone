import 'package:admin_api_types/admin_api_types.dart';

import '../enums/enums.dart';
import '../types/types.dart';
import 'endpoint.dart';

class MessagesEndpoint extends Endpoint {
  MessagesEndpoint(super.dio);

  Future<ApiResponse<List<MessageOverview>>> getMessagesByParticipant({
    required String participant,
    required MessageType type,
    required int pageNumber,
    required int pageSize,
  }) => get(
    '/api/v1/Messages',
    query: {'participant': participant, 'type': type.name, 'PageNumber': pageNumber, 'PageSize': pageSize},
    transformer: (e) => (e as List).map(MessageOverview.fromJson).toList(),
  );
}
