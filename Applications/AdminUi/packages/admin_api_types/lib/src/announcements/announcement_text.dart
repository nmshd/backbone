import 'package:json_annotation/json_annotation.dart';

export 'create_announcement_response.dart';

part 'announcement_text.g.dart';

@JsonSerializable()
class AnnouncementText {
  final String language;
  final String title;
  final String body;

  AnnouncementText({
    required this.language,
    required this.title,
    required this.body,
  });

  factory AnnouncementText.fromJson(dynamic json) => _$AnnouncementTextFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AnnouncementTextToJson(this);
}
