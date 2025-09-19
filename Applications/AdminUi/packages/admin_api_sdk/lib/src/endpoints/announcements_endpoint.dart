import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class AnnouncementsEndpoint extends Endpoint {
  AnnouncementsEndpoint(super.dio);

  Future<ApiResponse<List<Announcement>>> getAnnouncements() =>
      get('/api/v2/Announcements', transformer: (e) => (e as List).map(Announcement.fromJson).toList());

  Future<ApiResponse<Announcement>> getAnnouncement(String announcementId) =>
      get('/api/v2/Announcements/$announcementId', transformer: Announcement.fromJson);

  Future<ApiResponse<CreateAnnouncementResponse>> createAnnouncement({
    required AnnouncementSeverity severity,
    required List<AnnouncementText> announcementTexts,
    required bool isSilent,
    String? expiresAt,
    List<String>? recipients,
    List<AnnouncementAction>? actions,
    String? iqlQuery,
  }) => post(
    '/api/v2/Announcements',
    data: {
      'expiresAt': expiresAt,
      'severity': severity.name,
      'texts': announcementTexts,
      'recipients': recipients,
      'isSilent': isSilent,
      'iqlQuery': iqlQuery,
      'actions': actions,
    },
    transformer: CreateAnnouncementResponse.fromJson,
  );

  Future<ApiResponse<void>> deleteAnnouncement(String announcementId) => delete(
    '/api/v2/Announcements/$announcementId',
    expectedStatus: 204,
    transformer: (e) {},
    allowEmptyResponse: true,
  );
}
