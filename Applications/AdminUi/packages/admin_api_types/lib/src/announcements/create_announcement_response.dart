import 'package:json_annotation/json_annotation.dart';

import 'announcement_text.dart';

part 'create_announcement_response.g.dart';

@JsonSerializable()
class CreateAnnouncementResponse {
  final DateTime? expiresAt;
  final String severity;
  final List<AnnouncementText> texts;
  final List<String>? recipients;

  CreateAnnouncementResponse({required this.severity, required this.texts, this.recipients, this.expiresAt});

  factory CreateAnnouncementResponse.fromJson(dynamic json) => _$CreateAnnouncementResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$CreateAnnouncementResponseToJson(this);
}
