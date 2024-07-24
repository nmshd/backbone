// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_deletion_process_auditlog_entry.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IdentityDeletionProcessAuditLogEntry _$IdentityDeletionProcessAuditLogEntryFromJson(Map<String, dynamic> json) =>
    IdentityDeletionProcessAuditLogEntry(
      id: json['id'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      messageKey: json['messageKey'] as String,
      newStatus: json['newStatus'] as String,
      additionalData: Map<String, String>.from(json['additionalData'] as Map),
      oldStatus: json['oldStatus'] as String?,
    );

Map<String, dynamic> _$IdentityDeletionProcessAuditLogEntryToJson(IdentityDeletionProcessAuditLogEntry instance) => <String, dynamic>{
      'id': instance.id,
      'createdAt': instance.createdAt.toIso8601String(),
      'messageKey': instance.messageKey,
      'oldStatus': instance.oldStatus,
      'newStatus': instance.newStatus,
      'additionalData': instance.additionalData,
    };

AdditionalData _$AdditionalDataFromJson(Map<String, dynamic> json) => AdditionalData(
      key: json['key'] as String,
      value: json['value'] as String,
    );

Map<String, dynamic> _$AdditionalDataToJson(AdditionalData instance) => <String, dynamic>{
      'key': instance.key,
      'value': instance.value,
    };
