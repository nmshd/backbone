import 'package:json_annotation/json_annotation.dart';

import 'announcement_text.dart';

part 'create_announcement.g.dart';

@JsonSerializable()
class CreateAnnouncement {
  final DateTime? expiresAt;
  final String severity;
  final List<AnnouncementText> texts;

  CreateAnnouncement({
    required this.expiresAt,
    required this.severity,
    required this.texts,
  });

  factory CreateAnnouncement.fromJson(dynamic json) => _$CreateAnnouncementFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$CreateAnnouncementToJson(this);
}
