// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'announcement_text.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AnnouncementText _$AnnouncementTextFromJson(Map<String, dynamic> json) => AnnouncementText(
      announcementId: (json['announcementId'] as num).toInt(),
      languageId: (json['languageId'] as num).toInt(),
      title: json['title'] as String,
      body: json['body'] as String,
    );

Map<String, dynamic> _$AnnouncementTextToJson(AnnouncementText instance) => <String, dynamic>{
      'announcementId': instance.announcementId,
      'languageId': instance.languageId,
      'title': instance.title,
      'body': instance.body,
    };
