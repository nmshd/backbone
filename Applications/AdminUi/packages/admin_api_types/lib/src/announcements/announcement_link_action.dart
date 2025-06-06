import 'package:json_annotation/json_annotation.dart';

export 'create_announcement_response.dart';

part 'announcement_link_action.g.dart';

@JsonSerializable()
class AnnouncementLinkAction {
  String link;
  final Map<String, String> displayName;

  AnnouncementLinkAction({required this.link, required this.displayName});

  factory AnnouncementLinkAction.fromJson(dynamic json) => _$AnnouncementLinkActionFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AnnouncementLinkActionToJson(this);
}
