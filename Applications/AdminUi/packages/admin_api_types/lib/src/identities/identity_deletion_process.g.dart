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
  createdByDevice: json['createdByDevice'] as String?,
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
  'createdByDevice': instance.createdByDevice,
  'gracePeriodEndsAt': instance.gracePeriodEndsAt?.toIso8601String(),
  'gracePeriodReminder1SentAt': instance.gracePeriodReminder1SentAt?.toIso8601String(),
  'gracePeriodReminder2SentAt': instance.gracePeriodReminder2SentAt?.toIso8601String(),
  'gracePeriodReminder3SentAt': instance.gracePeriodReminder3SentAt?.toIso8601String(),
};

const _$DeletionProcessStatusEnumMap = {
  DeletionProcessStatus.active: 'Active',
  DeletionProcessStatus.cancelled: 'Cancelled',
  DeletionProcessStatus.deleting: 'Deleting',
};
