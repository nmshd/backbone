// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'announcements_overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AnnouncementOverview _$AnnouncementOverviewFromJson(Map<String, dynamic> json) => AnnouncementOverview(
      englishTitle: json['englishTitle'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      expiresAt: DateTime.parse(json['expiresAt'] as String),
      severity: json['severity'] as String,
      announcementTexts: (json['announcementTexts'] as List<dynamic>).map(AnnouncementText.fromJson).toList(),
    );

Map<String, dynamic> _$AnnouncementOverviewToJson(AnnouncementOverview instance) => <String, dynamic>{
      'englishTitle': instance.englishTitle,
      'createdAt': instance.createdAt.toIso8601String(),
      'expiresAt': instance.expiresAt.toIso8601String(),
      'severity': instance.severity,
      'announcementTexts': instance.announcementTexts,
    };
