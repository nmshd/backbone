import 'package:json_annotation/json_annotation.dart';

import 'announcement_text.dart';

part 'create_announcement.g.dart';

@JsonSerializable()
class CreateAnnouncement {
  final int id;
  final DateTime createAt;
  final DateTime expiresAt;
  final String severity;
  final AnnouncementText announcementText;

  CreateAnnouncement({
    required this.id,
    required this.createAt,
    required this.expiresAt,
    required this.severity,
    required this.announcementText,
  });

  factory CreateAnnouncement.fromJson(dynamic json) => _$CreateAnnouncementFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$CreateAnnouncementToJson(this);
}
