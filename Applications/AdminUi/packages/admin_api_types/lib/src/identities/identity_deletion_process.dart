import 'package:json_annotation/json_annotation.dart';

import '../enums/enums.dart';

part 'identity_deletion_process.g.dart';

@JsonSerializable()
class IdentityDeletionProcess {
  final String id;
  final DeletionProcessStatus status;
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
