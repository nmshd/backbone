// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deletion_process.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DeletionProcessDetail _$DeletionProcessDetailFromJson(Map<String, dynamic> json) => DeletionProcessDetail(
      id: json['id'] as String,
      status: json['status'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      approvalPeriodEndsAt: DateTime.parse(json['approvalPeriodEndsAt'] as String),
      approvalReminder1SentAt: json['approvalReminder1SentAt'] == null ? null : DateTime.parse(json['approvalReminder1SentAt'] as String),
      approvalReminder2SentAt: json['approvalReminder2SentAt'] == null ? null : DateTime.parse(json['approvalReminder2SentAt'] as String),
      approvalReminder3SentAt: json['approvalReminder3SentAt'] == null ? null : DateTime.parse(json['approvalReminder3SentAt'] as String),
      approvedAt: json['approvedAt'] == null ? null : DateTime.parse(json['approvedAt'] as String),
      approvedByDevice: json['approvedByDevice'] as String?,
      gracePeriodEndsAt: json['gracePeriodEndsAt'] == null ? null : DateTime.parse(json['gracePeriodEndsAt'] as String),
      gracePeriodReminder1SentAt: json['gracePeriodReminder1SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder1SentAt'] as String),
      gracePeriodReminder2SentAt: json['gracePeriodReminder2SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder2SentAt'] as String),
      gracePeriodReminder3SentAt: json['gracePeriodReminder3SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder3SentAt'] as String),
      auditLogs: (json['auditLogs'] as List<dynamic>?)?.map(AuditLog.fromJson).toList(),
    );

Map<String, dynamic> _$DeletionProcessDetailToJson(DeletionProcessDetail instance) => <String, dynamic>{
      'id': instance.id,
      'status': instance.status,
      'createdAt': instance.createdAt.toIso8601String(),
      'approvalPeriodEndsAt': instance.approvalPeriodEndsAt.toIso8601String(),
      'approvalReminder1SentAt': instance.approvalReminder1SentAt?.toIso8601String(),
      'approvalReminder2SentAt': instance.approvalReminder2SentAt?.toIso8601String(),
      'approvalReminder3SentAt': instance.approvalReminder3SentAt?.toIso8601String(),
      'approvedAt': instance.approvedAt?.toIso8601String(),
      'approvedByDevice': instance.approvedByDevice,
      'gracePeriodEndsAt': instance.gracePeriodEndsAt?.toIso8601String(),
      'gracePeriodReminder1SentAt': instance.gracePeriodReminder1SentAt?.toIso8601String(),
      'gracePeriodReminder2SentAt': instance.gracePeriodReminder2SentAt?.toIso8601String(),
      'gracePeriodReminder3SentAt': instance.gracePeriodReminder3SentAt?.toIso8601String(),
      'auditLogs': instance.auditLogs,
    };

DeletionProcess _$DeletionProcessFromJson(Map<String, dynamic> json) => DeletionProcess(
      id: json['id'] as String,
      status: json['status'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      approvalPeriodEndsAt: DateTime.parse(json['approvalPeriodEndsAt'] as String),
      approvalReminder1SentAt: json['approvalReminder1SentAt'] == null ? null : DateTime.parse(json['approvalReminder1SentAt'] as String),
      approvalReminder2SentAt: json['approvalReminder2SentAt'] == null ? null : DateTime.parse(json['approvalReminder2SentAt'] as String),
      approvalReminder3SentAt: json['approvalReminder3SentAt'] == null ? null : DateTime.parse(json['approvalReminder3SentAt'] as String),
      approvedAt: json['approvedAt'] == null ? null : DateTime.parse(json['approvedAt'] as String),
      approvedByDevice: json['approvedByDevice'] as String?,
      gracePeriodEndsAt: json['gracePeriodEndsAt'] == null ? null : DateTime.parse(json['gracePeriodEndsAt'] as String),
      gracePeriodReminder1SentAt: json['gracePeriodReminder1SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder1SentAt'] as String),
      gracePeriodReminder2SentAt: json['gracePeriodReminder2SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder2SentAt'] as String),
      gracePeriodReminder3SentAt: json['gracePeriodReminder3SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder3SentAt'] as String),
    );

Map<String, dynamic> _$DeletionProcessToJson(DeletionProcess instance) => <String, dynamic>{
      'id': instance.id,
      'status': instance.status,
      'createdAt': instance.createdAt.toIso8601String(),
      'approvalPeriodEndsAt': instance.approvalPeriodEndsAt.toIso8601String(),
      'approvalReminder1SentAt': instance.approvalReminder1SentAt?.toIso8601String(),
      'approvalReminder2SentAt': instance.approvalReminder2SentAt?.toIso8601String(),
      'approvalReminder3SentAt': instance.approvalReminder3SentAt?.toIso8601String(),
      'approvedAt': instance.approvedAt?.toIso8601String(),
      'approvedByDevice': instance.approvedByDevice,
      'gracePeriodEndsAt': instance.gracePeriodEndsAt?.toIso8601String(),
      'gracePeriodReminder1SentAt': instance.gracePeriodReminder1SentAt?.toIso8601String(),
      'gracePeriodReminder2SentAt': instance.gracePeriodReminder2SentAt?.toIso8601String(),
      'gracePeriodReminder3SentAt': instance.gracePeriodReminder3SentAt?.toIso8601String(),
    };

AuditLog _$AuditLogFromJson(Map<String, dynamic> json) => AuditLog(
      id: json['id'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      messageKey: json['messageKey'] as String,
      newStatus: json['newStatus'] as String,
      additionalData: Map<String, String>.from(json['additionalData'] as Map),
      oldStatus: json['oldStatus'] as String?,
    );

Map<String, dynamic> _$AuditLogToJson(AuditLog instance) => <String, dynamic>{
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
