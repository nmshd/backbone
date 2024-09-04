// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_announcement.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreateAnnouncement _$CreateAnnouncementFromJson(Map<String, dynamic> json) => CreateAnnouncement(
      id: (json['id'] as num).toInt(),
      createAt: DateTime.parse(json['createAt'] as String),
      expiresAt: DateTime.parse(json['expiresAt'] as String),
      severity: json['severity'] as String,
      announcementText: AnnouncementText.fromJson(json['announcementText']),
    );

Map<String, dynamic> _$CreateAnnouncementToJson(CreateAnnouncement instance) => <String, dynamic>{
      'id': instance.id,
      'createAt': instance.createAt.toIso8601String(),
      'expiresAt': instance.expiresAt.toIso8601String(),
      'severity': instance.severity,
      'announcementText': instance.announcementText,
    };
