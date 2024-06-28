// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deletion_process.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AuditLogs _$AuditLogsFromJson(Map<String, dynamic> json) => AuditLogs(
      id: json['id'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      messageKey: json['messageKey'] as String,
      newStatus: json['newStatus'] as String,
      oldStatus: json['oldStatus'] as String,
    );

Map<String, dynamic> _$AuditLogsToJson(AuditLogs instance) => <String, dynamic>{
      'id': instance.id,
      'createdAt': instance.createdAt.toIso8601String(),
      'messageKey': instance.messageKey,
      'oldStatus': instance.oldStatus,
      'newStatus': instance.newStatus,
    };
