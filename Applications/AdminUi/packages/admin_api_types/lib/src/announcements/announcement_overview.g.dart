// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'announcement_overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Announcement _$AnnouncementFromJson(Map<String, dynamic> json) => Announcement(
  id: json['id'] as String,
  createdAt: DateTime.parse(json['createdAt'] as String),
  expiresAt: json['expiresAt'] == null ? null : DateTime.parse(json['expiresAt'] as String),
  severity: json['severity'] as String,
  texts: (json['texts'] as List<dynamic>).map(AnnouncementText.fromJson).toList(),
  actions: (json['actions'] as List<dynamic>).map(AnnouncementAction.fromJson).toList(),
  iqlQuery: json['iqlQuery'] as String?,
  isSilent: json['isSilent'] as bool,
);

Map<String, dynamic> _$AnnouncementToJson(Announcement instance) => <String, dynamic>{
  'id': instance.id,
  'createdAt': instance.createdAt.toIso8601String(),
  'expiresAt': instance.expiresAt?.toIso8601String(),
  'severity': instance.severity,
  'texts': instance.texts,
  'actions': instance.actions,
  'iqlQuery': instance.iqlQuery,
  'isSilent': instance.isSilent,
};
