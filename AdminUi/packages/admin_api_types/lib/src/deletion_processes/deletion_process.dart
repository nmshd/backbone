import 'package:json_annotation/json_annotation.dart';

part 'deletion_process.g.dart';

@JsonSerializable()
class AuditLogs {
  final String id;
  final DateTime createdAt;
  final String messageKey;
  final String? oldStatus;
  final String newStatus;

  AuditLogs({
    required this.id,
    required this.createdAt,
    required this.messageKey,
    required this.newStatus,
    this.oldStatus,
  });

  factory AuditLogs.fromJson(dynamic json) => _$AuditLogsFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AuditLogsToJson(this);
}
