import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class AnnouncementsEndpoint extends Endpoint {
  AnnouncementsEndpoint(super.dio);

  Future<ApiResponse<List<Announcement>>> getAnnouncements() => get(
        '/api/v1/Announcements',
        transformer: (e) => (e as List).map(Announcement.fromJson).toList(),
      );

  Future<ApiResponse<Announcement>> getAnnouncement(String announcementId) => get(
        '/api/v1/Announcements/$announcementId',
        transformer: Announcement.fromJson,
      );

  Future<ApiResponse<CreateAnnouncementResponse>> createAnnouncement({
    required AnnouncementSeverity severity,
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
