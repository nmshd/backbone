import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class AnnouncementsEndpoint extends Endpoint {
  AnnouncementsEndpoint(super.dio);

  Future<ApiResponse<List<AnnouncementOverview>>> getAnnouncements() => get(
        '/api/v1/Announcements',
        transformer: (e) => (e as List).map(AnnouncementOverview.fromJson).toList(),
      );

  Future<ApiResponse<AnnouncementOverview>> getAnnouncement(String announcementId) => get(
        '/api/v1/Announcements/$announcementId',
        transformer: AnnouncementOverview.fromJson,
      );

  Future<ApiResponse<CreateAnnouncementResponse>> createAnnouncement({
    required String severity,
    required List<AnnouncementText> announcementTexts,
    String? expiresAt,
    List<String>? recipients,
  }) =>
      post(
        '/api/v1/Announcements',
        data: {'expiresAt': expiresAt, 'severity': severity, 'texts': announcementTexts, 'recipients': recipients},
        transformer: CreateAnnouncementResponse.fromJson,
      );
}
