import 'package:admin_api_types/src/admin_api_types_base.dart';
import 'package:json_annotation/json_annotation.dart';

part 'identity_deletion_process.g.dart';

@JsonSerializable()
class IdentityDeletionProcess {
  final String id;
  final String status;
  final DateTime createdAt;
  final DateTime approvalPeriodEndsAt;
  final DateTime? approvalReminder1SentAt;
  final DateTime? approvalReminder2SentAt;
  final DateTime? approvalReminder3SentAt;
  final DateTime? approvedAt;
  final String? approvedByDevice;
  final DateTime? gracePeriodEndsAt;
  final DateTime? gracePeriodReminder1SentAt;
  final DateTime? gracePeriodReminder2SentAt;
  final DateTime? gracePeriodReminder3SentAt;

  IdentityDeletionProcess({
    required this.id,
    required this.status,
    required this.createdAt,
    required this.approvalPeriodEndsAt,
    this.approvalReminder1SentAt,
    this.approvalReminder2SentAt,
    this.approvalReminder3SentAt,
    this.approvedAt,
    this.approvedByDevice,
    this.gracePeriodEndsAt,
    this.gracePeriodReminder1SentAt,
    this.gracePeriodReminder2SentAt,
    this.gracePeriodReminder3SentAt,
  });

  factory IdentityDeletionProcess.fromJson(dynamic json) => _$IdentityDeletionProcessFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityDeletionProcessToJson(this);
}

@JsonSerializable()
class IdentityDeletionProcessDetail {
  final String id;
  final String status;
  final DateTime createdAt;
  final List<IdentityDeletionProcessAuditLogEntry> auditLog;

  IdentityDeletionProcessDetail({
    required this.id,
    required this.status,
    required this.createdAt,
    required this.auditLog,
  });

  factory IdentityDeletionProcessDetail.fromJson(dynamic json) => _$IdentityDeletionProcessDetailFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityDeletionProcessDetailToJson(this);
}
