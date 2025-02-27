// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_deletion_process_audit_log_entry.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IdentityDeletionProcessAuditLogEntry _$IdentityDeletionProcessAuditLogEntryFromJson(Map<String, dynamic> json) =>
    IdentityDeletionProcessAuditLogEntry(
      id: json['id'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      messageKey: json['messageKey'] as String,
      newStatus: json['newStatus'] as String,
      additionalData: (json['additionalData'] as Map<String, dynamic>?)?.map((k, e) => MapEntry(k, e as String)) ?? const {},
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
