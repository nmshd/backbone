import 'package:json_annotation/json_annotation.dart';

part 'message.g.dart';

@JsonSerializable()
class MessageOverview {
  final String messageId;
  final String senderAddress;
  final String senderDevice;
  final DateTime sendDate;
  final int numberOfAttachments;
  final List<MessageRecipients> recipients;

  MessageOverview({
    required this.messageId,
    required this.senderAddress,
    required this.senderDevice,
    required this.sendDate,
    required this.numberOfAttachments,
    required this.recipients,
  });

  factory MessageOverview.fromJson(dynamic json) => _$MessageOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MessageOverviewToJson(this);
}

@JsonSerializable()
class MessageRecipients {
  final String messageId;
  final String address;

  MessageRecipients({
    required this.messageId,
    required this.address,
  });

  factory MessageRecipients.fromJson(dynamic json) => _$MessageRecipientsFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MessageRecipientsToJson(this);
}
