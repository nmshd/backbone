import 'package:admin_api_types/admin_api_types.dart';
import 'package:json_annotation/json_annotation.dart';

part 'identity.g.dart';

@JsonSerializable()
class Identity {
  final String address;
  final String clientId;
  final String publicKey;
  final String tierId;
  final DateTime createdAt;
  final int identityVersion;
  final int numberOfDevices;
  final List<Device>? devices;
  final List<IdentityQuota>? quotas;

  Identity({
    required this.address,
    required this.clientId,
    required this.publicKey,
    required this.tierId,
    required this.createdAt,
    required this.identityVersion,
    required this.numberOfDevices,
    this.devices,
    this.quotas,
  });

  factory Identity.fromJson(dynamic json) => _$IdentityFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityToJson(this);
}

@JsonSerializable()
class Device {
  final String id;
  final String username;
  final DateTime createdAt;
  final String createdByDevice;
  final Map<String, dynamic>? lastLogin;

  Device({
    required this.id,
    required this.username,
    required this.createdAt,
    required this.createdByDevice,
    this.lastLogin,
  });

  factory Device.fromJson(dynamic json) => _$DeviceFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeviceToJson(this);
}

@JsonSerializable()
class IdentityQuota {
  final String id;
  final String source;
  final Metric metric;
  final int max;
  final int usage;
  final String period;

  IdentityQuota({
    required this.id,
    required this.source,
    required this.metric,
    required this.max,
    required this.usage,
    required this.period,
  });

  factory IdentityQuota.fromJson(dynamic json) => _$IdentityQuotaFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityQuotaToJson(this);
}

@JsonSerializable()
class IdentityOverview {
  final String address;
  final DateTime createdAt;
  final String createdWithClient;
  final int identityVersion;
  final int numberOfDevices;
  final Tier tier;
  final DateTime? lastLoginAt;
  final int? datawalletVersion;

  IdentityOverview({
    required this.address,
    required this.createdAt,
    required this.createdWithClient,
    required this.identityVersion,
    required this.numberOfDevices,
    required this.tier,
    this.lastLoginAt,
    this.datawalletVersion,
  });

  factory IdentityOverview.fromJson(dynamic json) => _$IdentityOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityOverviewToJson(this);
}

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
  final List<AuditLog>? auditLogs;

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
    this.auditLogs,
  });

  factory DeletionProcess.fromJson(dynamic json) => _$DeletionProcessFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeletionProcessToJson(this);
}

@JsonSerializable()
class AuditLog {
  final String id;
  final DateTime createdAt;
  final String? oldStatus;
  final String newStatus;
  final Map<String, dynamic>? additionalData;
  final String messageKey;

  AuditLog({
    required this.id,
    required this.createdAt,
    required this.newStatus,
    required this.messageKey,
    this.oldStatus,
    this.additionalData,
  });

  factory AuditLog.fromJson(dynamic json) => _$AuditLogFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$AuditLogToJson(this);
}
