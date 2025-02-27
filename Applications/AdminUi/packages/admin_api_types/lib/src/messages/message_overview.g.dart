// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'message_overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MessageOverview _$MessageOverviewFromJson(Map<String, dynamic> json) => MessageOverview(
  messageId: json['messageId'] as String,
  senderAddress: json['senderAddress'] as String,
  senderDevice: json['senderDevice'] as String,
  sendDate: DateTime.parse(json['sendDate'] as String),
  numberOfAttachments: (json['numberOfAttachments'] as num).toInt(),
  recipients: (json['recipients'] as List<dynamic>).map(MessageRecipient.fromJson).toList(),
);

Map<String, dynamic> _$MessageOverviewToJson(MessageOverview instance) => <String, dynamic>{
  'messageId': instance.messageId,
  'senderAddress': instance.senderAddress,
  'senderDevice': instance.senderDevice,
  'sendDate': instance.sendDate.toIso8601String(),
  'numberOfAttachments': instance.numberOfAttachments,
  'recipients': instance.recipients,
};
