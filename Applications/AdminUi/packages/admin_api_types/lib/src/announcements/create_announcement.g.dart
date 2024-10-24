// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_announcement.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreateAnnouncement _$CreateAnnouncementFromJson(Map<String, dynamic> json) => CreateAnnouncement(
      expiresAt: DateTime.parse(json['expiresAt'] as String),
      severity: json['severity'] as String,
      announcementTexts: (json['announcementTexts'] as List<dynamic>).map(AnnouncementText.fromJson).toList(),
    );

Map<String, dynamic> _$CreateAnnouncementToJson(CreateAnnouncement instance) => <String, dynamic>{
      'expiresAt': instance.expiresAt.toIso8601String(),
      'severity': instance.severity,
      'announcementTexts': instance.announcementTexts,
    };
