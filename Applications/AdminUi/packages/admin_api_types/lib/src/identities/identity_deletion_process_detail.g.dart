// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_deletion_process_detail.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IdentityDeletionProcessDetail _$IdentityDeletionProcessDetailFromJson(Map<String, dynamic> json) => IdentityDeletionProcessDetail(
  id: json['id'] as String,
  status: $enumDecode(_$DeletionProcessStatusEnumMap, json['status']),
  createdAt: DateTime.parse(json['createdAt'] as String),
  auditLog: (json['auditLog'] as List<dynamic>).map(IdentityDeletionProcessAuditLogEntry.fromJson).toList(),
);

Map<String, dynamic> _$IdentityDeletionProcessDetailToJson(IdentityDeletionProcessDetail instance) => <String, dynamic>{
  'id': instance.id,
  'status': _$DeletionProcessStatusEnumMap[instance.status]!,
  'createdAt': instance.createdAt.toIso8601String(),
  'auditLog': instance.auditLog,
};

const _$DeletionProcessStatusEnumMap = {
  DeletionProcessStatus.waitingForApproval: 'WaitingForApproval',
  DeletionProcessStatus.approved: 'Approved',
  DeletionProcessStatus.cancelled: 'Cancelled',
  DeletionProcessStatus.rejected: 'Rejected',
  DeletionProcessStatus.deleting: 'Deleting',
};
