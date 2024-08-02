import 'package:json_annotation/json_annotation.dart';

part 'message_overview.g.dart';

@JsonSerializable()
class MessageOverview {
  final String messageId;
  final String senderAddress;
  final String senderDevice;
  final DateTime sendDate;
  final int numberOfAttachments;
  final List<MessageRecipient> recipients;

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
class MessageRecipient {
  final String address;

  MessageRecipient({
    required this.address,
  });

  factory MessageRecipient.fromJson(dynamic json) => _$MessageRecipientFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MessageRecipientToJson(this);
}
