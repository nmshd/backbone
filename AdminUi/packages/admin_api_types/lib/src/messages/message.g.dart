// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'message.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Message _$MessageFromJson(Map<String, dynamic> json) => Message(
      messageId: json['messageId'] as String,
      senderAddress: json['senderAddress'] as String,
      senderDevice: json['senderDevice'] as String,
      sendDate: DateTime.parse(json['sendDate'] as String),
      numberOfAttachments: (json['numberOfAttachments'] as num).toInt(),
      recipients: (json['recipients'] as List<dynamic>).map(MessageRecipient.fromJson).toList(),
    );

Map<String, dynamic> _$MessageToJson(Message instance) => <String, dynamic>{
      'messageId': instance.messageId,
      'senderAddress': instance.senderAddress,
      'senderDevice': instance.senderDevice,
      'sendDate': instance.sendDate.toIso8601String(),
      'numberOfAttachments': instance.numberOfAttachments,
      'recipients': instance.recipients,
    };

MessageRecipient _$MessageRecipientFromJson(Map<String, dynamic> json) => MessageRecipient(
      address: json['address'] as String,
    );

Map<String, dynamic> _$MessageRecipientToJson(MessageRecipient instance) => <String, dynamic>{
      'address': instance.address,
    };
