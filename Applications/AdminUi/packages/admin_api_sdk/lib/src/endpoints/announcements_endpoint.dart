import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class AnnouncementsEndpoint extends Endpoint {
  AnnouncementsEndpoint(super.dio);

  Future<ApiResponse<List<AnnouncementOverview>>> getAnnouncements() => get(
        '/api/v1/Announcements',
        transformer: (e) => (e as List).map(AnnouncementOverview.fromJson).toList(),
      );

  Future<ApiResponse<CreateAnnouncement>> createAnnouncement({
    required String? expiresAt,
    required String severity,
    required List<AnnouncementText> announcementTexts,
  }) =>
      post(
        '/api/v1/Announcements',
        data: {'expiresAt': expiresAt, 'severity': severity, 'texts': announcementTexts},
        transformer: CreateAnnouncement.fromJson,
      );
}
