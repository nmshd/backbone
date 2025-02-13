import 'package:json_annotation/json_annotation.dart';

import 'deletion_process_status.dart';
import 'identity_deletion_process_audit_log_entry.dart';

part 'identity_deletion_process_detail.g.dart';

@JsonSerializable()
class IdentityDeletionProcessDetail {
  final String id;
  final DeletionProcessStatus status;
  final DateTime createdAt;
  final List<IdentityDeletionProcessAuditLogEntry> auditLog;

  IdentityDeletionProcessDetail({required this.id, required this.status, required this.createdAt, required this.auditLog});

  factory IdentityDeletionProcessDetail.fromJson(dynamic json) => _$IdentityDeletionProcessDetailFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityDeletionProcessDetailToJson(this);
}
