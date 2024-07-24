import 'package:json_annotation/json_annotation.dart';

part 'identity_deletion_process_auditlog_entry.g.dart';

@JsonSerializable()
class IdentityDeletionProcessAuditLogEntry {
  final String id;
  final DateTime createdAt;
  final String messageKey;
  final String? oldStatus;
  final String newStatus;
  final Map<String, String> additionalData;

  IdentityDeletionProcessAuditLogEntry({
    required this.id,
    required this.createdAt,
    required this.messageKey,
    required this.newStatus,
    required this.additionalData,
    this.oldStatus,
  });

  factory IdentityDeletionProcessAuditLogEntry.fromJson(dynamic json) => _$IdentityDeletionProcessAuditLogEntryFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityDeletionProcessAuditLogEntryToJson(this);
}
