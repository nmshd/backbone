import 'package:json_annotation/json_annotation.dart';

import 'announcement_text.dart';

export 'create_announcement.dart';

part 'announcements_overview.g.dart';

@JsonSerializable()
class AnnouncementOverview {
  final String englishTitle;
  final DateTime createdAt;
  final DateTime expiresAt;
  final String severity;
  final List<AnnouncementText> announcementTexts;

  AnnouncementOverview({
    required this.englishTitle,
    required this.createdAt,
    required this.expiresAt,
    required this.severity,
    required this.announcementTexts,
  });

  factory AnnouncementOverview.fromJson(dynamic json) => _$AnnouncementOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AnnouncementOverviewToJson(this);
}
