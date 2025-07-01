// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'announcement_action.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AnnouncementAction _$AnnouncementActionFromJson(Map<String, dynamic> json) => AnnouncementAction(
  link: json['link'] as String,
  displayName: Map<String, String>.from(json['displayName'] as Map),
);

Map<String, dynamic> _$AnnouncementActionToJson(AnnouncementAction instance) => <String, dynamic>{
  'link': instance.link,
  'displayName': instance.displayName,
};
