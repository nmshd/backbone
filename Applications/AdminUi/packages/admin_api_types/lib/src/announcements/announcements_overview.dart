import 'package:admin_api_types/admin_api_types.dart';
import 'package:json_annotation/json_annotation.dart';

export 'create_announcement_response.dart';

part 'announcements_overview.g.dart';

@JsonSerializable()
class Announcement {
  final String id;
  final DateTime createdAt;
  final DateTime? expiresAt;
  final String severity;
  final List<AnnouncementText> texts;
  final List<AnnouncementLinkAction> actions;
  final String? iqlQuery;
  final bool isSilent;

  Announcement({
    required this.id,
    required this.createdAt,
    required this.expiresAt,
    required this.severity,
    required this.texts,
    required this.actions,
    required this.iqlQuery,
    required this.isSilent,
  });

  factory Announcement.fromJson(dynamic json) => _$AnnouncementFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AnnouncementToJson(this);
}
