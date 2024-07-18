import 'package:json_annotation/json_annotation.dart';

part 'deletion_process.g.dart';

@JsonSerializable()
class DeletionProcess {
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

  DeletionProcess({
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

  factory DeletionProcess.fromJson(dynamic json) => _$DeletionProcessFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeletionProcessToJson(this);
}

@JsonSerializable()
class DeletionProcessDetail {
  final String id;
  final String status;
  final DateTime createdAt;
  final List<AuditLog> auditLog;

  DeletionProcessDetail({
    required this.id,
    required this.status,
    required this.createdAt,
    required this.auditLog,
  });

  factory DeletionProcessDetail.fromJson(dynamic json) => _$DeletionProcessDetailFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeletionProcessDetailToJson(this);
}

@JsonSerializable()
class AuditLog {
  final String id;
  final DateTime createdAt;
  final String messageKey;
  final String? oldStatus;
  final String newStatus;
  final Map<String, String> additionalData;

  AuditLog({
    required this.id,
    required this.createdAt,
    required this.messageKey,
    required this.newStatus,
    required this.additionalData,
    this.oldStatus,
  });

  factory AuditLog.fromJson(dynamic json) => _$AuditLogFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AuditLogToJson(this);
}

@JsonSerializable()
class AdditionalData {
  final String key;
  final String value;

  AdditionalData({
    required this.key,
    required this.value,
  });

  factory AdditionalData.fromJson(dynamic json) => _$AdditionalDataFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AdditionalDataToJson(this);
}
