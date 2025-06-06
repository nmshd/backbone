import 'package:json_annotation/json_annotation.dart';

import 'announcement_text.dart';

export 'create_announcement_response.dart';

part 'announcements_overview.g.dart';

@JsonSerializable()
class Announcement {
  final String id;
  final DateTime createdAt;
  final DateTime? expiresAt;
  final String severity;
  final List<AnnouncementText> texts;
  final String? iqlQuery;

  Announcement({
    required this.id,
    required this.createdAt,
    required this.expiresAt,
    required this.severity,
    required this.texts,
    required this.iqlQuery,
  });

  factory Announcement.fromJson(dynamic json) => _$AnnouncementFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AnnouncementToJson(this);
}
