import 'package:json_annotation/json_annotation.dart';

part 'deletion_process.g.dart';

@JsonSerializable()
class DeletionProcessOverview {
  final String id;
  final String status;
  final DateTime createdAt;
  final DateTime? approvalReminder1SentAt;
  final DateTime? approvalReminder2SentAt;
  final DateTime? approvalReminder3SentAt;
  final DateTime? approvedAt;
  final Map<String, dynamic>? approvedByDevice;
  final DateTime? gracePeriodEndsAt;
  final DateTime? gracePeriodReminder1SentAt;
  final DateTime? gracePeriodReminder2SentAt;
  final DateTime? gracePeriodReminder3SentAt;

  DeletionProcessOverview({
    required this.id,
    required this.status,
    required this.createdAt,
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

  factory DeletionProcessOverview.fromJson(dynamic json) => _$DeletionProcessOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeletionProcessOverviewToJson(this);
}

@JsonSerializable()
class DeletionProcessDetails {
  final String id;
  final List<AuditLog> auditLog;
  final String status;
  final DateTime createdAt;
  final DateTime? approvalReminder1SentAt;
  final DateTime? approvalReminder2SentAt;
  final DateTime? approvalReminder3SentAt;
  final DateTime? approvedAt;
  final Map<String, dynamic>? approvedByDevice;
  final DateTime? gracePeriodEndsAt;
  final DateTime? gracePeriodReminder1SentAt;
  final DateTime? gracePeriodReminder2SentAt;
  final DateTime? gracePeriodReminder3SentAt;

  DeletionProcessDetails({
    required this.id,
    required this.auditLog,
    required this.status,
    required this.createdAt,
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

  factory DeletionProcessDetails.fromJson(dynamic json) => _$DeletionProcessDetailsFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeletionProcessDetailsToJson(this);
}

@JsonSerializable()
class AuditLog {
  final String id;
  final DateTime createdAt;
  final String message;
  final String? oldStatus;
  final String newStatus;

  AuditLog({
    required this.id,
    required this.createdAt,
    required this.message,
    required this.newStatus,
    this.oldStatus,
  });

  factory AuditLog.fromJson(dynamic json) => _$AuditLogFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AuditLogToJson(this);
}
