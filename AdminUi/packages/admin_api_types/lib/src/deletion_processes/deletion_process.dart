import 'package:json_annotation/json_annotation.dart';

part 'deletion_process.g.dart';

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
