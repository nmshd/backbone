import 'package:json_annotation/json_annotation.dart';

export 'create_announcement_response.dart';

part 'announcement_action.g.dart';

@JsonSerializable()
class AnnouncementAction {
  final String link;
  final Map<String, String> displayName;

  AnnouncementAction({required this.link, required this.displayName});

  factory AnnouncementAction.fromJson(dynamic json) => _$AnnouncementActionFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AnnouncementActionToJson(this);
}
