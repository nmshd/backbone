import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class AnnouncementsEndpoint extends Endpoint {
  AnnouncementsEndpoint(super.dio);

  Future<ApiResponse<List<Announcement>>> getAnnouncements() =>
      get('/api/v1/Announcements', transformer: (e) => (e as List).map(Announcement.fromJson).toList());

  Future<ApiResponse<Announcement>> getAnnouncement(String announcementId) =>
      get('/api/v1/Announcements/$announcementId', transformer: Announcement.fromJson);

  Future<ApiResponse<CreateAnnouncementResponse>> createAnnouncement({
    required AnnouncementSeverity severity,
    required List<AnnouncementText> announcementTexts,
    required bool isSilent,
    String? expiresAt,
    List<String>? recipients,
    String? iqlQuery,
  }) => post(
    '/api/v1/Announcements',
    data: {
      'expiresAt': expiresAt,
      'severity': severity.name,
      'texts': announcementTexts,
      'recipients': recipients,
      'isSilent': isSilent,
      'iqlQuery': iqlQuery,
    },
    transformer: CreateAnnouncementResponse.fromJson,
  );
}
