// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_deletion_process.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IdentityDeletionProcess _$IdentityDeletionProcessFromJson(
  Map<String, dynamic> json,
) => IdentityDeletionProcess(
  id: json['id'] as String,
  status: $enumDecode(_$DeletionProcessStatusEnumMap, json['status']),
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

Map<String, dynamic> _$IdentityDeletionProcessToJson(
  IdentityDeletionProcess instance,
) => <String, dynamic>{
  'id': instance.id,
  'status': _$DeletionProcessStatusEnumMap[instance.status]!,
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

const _$DeletionProcessStatusEnumMap = {
  DeletionProcessStatus.waitingForApproval: 'WaitingForApproval',
  DeletionProcessStatus.approved: 'Approved',
  DeletionProcessStatus.cancelled: 'Cancelled',
  DeletionProcessStatus.rejected: 'Rejected',
  DeletionProcessStatus.deleting: 'Deleting',
};
