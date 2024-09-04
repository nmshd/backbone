import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class AnnouncementsEndpoint extends Endpoint {
  AnnouncementsEndpoint(super.dio);

  Future<ApiResponse<CreateAnnouncement>> createClient() => post(
        '/api/v1/Clients',
        data: {},
        transformer: CreateAnnouncement.fromJson,
      );
}
