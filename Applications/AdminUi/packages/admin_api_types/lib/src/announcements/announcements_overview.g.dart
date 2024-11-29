// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'announcements_overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AnnouncementOverview _$AnnouncementOverviewFromJson(Map<String, dynamic> json) => AnnouncementOverview(
      createdAt: DateTime.parse(json['createdAt'] as String),
      expiresAt: json['expiresAt'] == null ? null : DateTime.parse(json['expiresAt'] as String),
      severity: json['severity'] as String,
      texts: (json['texts'] as List<dynamic>).map(AnnouncementText.fromJson).toList(),
    );

Map<String, dynamic> _$AnnouncementOverviewToJson(AnnouncementOverview instance) => <String, dynamic>{
      'createdAt': instance.createdAt.toIso8601String(),
      'expiresAt': instance.expiresAt?.toIso8601String(),
      'severity': instance.severity,
      'texts': instance.texts,
    };
