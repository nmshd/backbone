import 'package:json_annotation/json_annotation.dart';

part 'message_recipient.g.dart';

@JsonSerializable()
class MessageRecipient {
  final String address;

  MessageRecipient({required this.address});

  factory MessageRecipient.fromJson(dynamic json) => _$MessageRecipientFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MessageRecipientToJson(this);
}
