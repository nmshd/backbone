// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_announcement_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreateAnnouncementResponse _$CreateAnnouncementResponseFromJson(
  Map<String, dynamic> json,
) => CreateAnnouncementResponse(
  severity: json['severity'] as String,
  texts: (json['texts'] as List<dynamic>).map(AnnouncementText.fromJson).toList(),
  recipients: (json['recipients'] as List<dynamic>?)?.map((e) => e as String).toList(),
  expiresAt: json['expiresAt'] == null ? null : DateTime.parse(json['expiresAt'] as String),
);

Map<String, dynamic> _$CreateAnnouncementResponseToJson(
  CreateAnnouncementResponse instance,
) => <String, dynamic>{
  'expiresAt': instance.expiresAt?.toIso8601String(),
  'severity': instance.severity,
  'texts': instance.texts,
  'recipients': instance.recipients,
};
