import 'package:json_annotation/json_annotation.dart';

import 'deletion_process_status.dart';

part 'identity_deletion_process.g.dart';

@JsonSerializable()
class IdentityDeletionProcess {
  final String id;
  final DeletionProcessStatus status;
  final DateTime createdAt;
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
