import 'package:json_annotation/json_annotation.dart';

export 'create_announcement.dart';

part 'announcement_text.g.dart';

@JsonSerializable()
class AnnouncementText {
  final int announcementId;
  final int languageId;
  final String title;
  final String body;

  AnnouncementText({
    required this.announcementId,
    required this.languageId,
    required this.title,
    required this.body,
  });

  factory AnnouncementText.fromJson(dynamic json) => _$AnnouncementTextFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AnnouncementTextToJson(this);
}
