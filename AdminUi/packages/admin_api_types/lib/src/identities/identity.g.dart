// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Identity _$IdentityFromJson(Map<String, dynamic> json) => Identity(
      address: json['address'] as String,
      clientId: json['clientId'] as String,
      publicKey: json['publicKey'] as String,
      tierId: json['tierId'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      identityVersion: (json['identityVersion'] as num).toInt(),
      numberOfDevices: (json['numberOfDevices'] as num).toInt(),
      devices: (json['devices'] as List<dynamic>?)?.map(Device.fromJson).toList(),
      quotas: (json['quotas'] as List<dynamic>?)?.map(IdentityQuota.fromJson).toList(),
    );

Map<String, dynamic> _$IdentityToJson(Identity instance) => <String, dynamic>{
      'address': instance.address,
      'clientId': instance.clientId,
      'publicKey': instance.publicKey,
      'tierId': instance.tierId,
      'createdAt': instance.createdAt.toIso8601String(),
      'identityVersion': instance.identityVersion,
      'numberOfDevices': instance.numberOfDevices,
      'devices': instance.devices,
      'quotas': instance.quotas,
    };

Device _$DeviceFromJson(Map<String, dynamic> json) => Device(
      id: json['id'] as String,
      username: json['username'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      createdByDevice: json['createdByDevice'] as String,
      lastLogin: json['lastLogin'] as Map<String, dynamic>?,
    );

Map<String, dynamic> _$DeviceToJson(Device instance) => <String, dynamic>{
      'id': instance.id,
      'username': instance.username,
      'createdAt': instance.createdAt.toIso8601String(),
      'createdByDevice': instance.createdByDevice,
      'lastLogin': instance.lastLogin,
    };

IdentityQuota _$IdentityQuotaFromJson(Map<String, dynamic> json) => IdentityQuota(
      id: json['id'] as String,
      source: json['source'] as String,
      metric: Metric.fromJson(json['metric']),
      max: (json['max'] as num).toInt(),
      usage: (json['usage'] as num).toInt(),
      period: json['period'] as String,
    );

Map<String, dynamic> _$IdentityQuotaToJson(IdentityQuota instance) => <String, dynamic>{
      'id': instance.id,
      'source': instance.source,
      'metric': instance.metric,
      'max': instance.max,
      'usage': instance.usage,
      'period': instance.period,
    };

IdentityOverview _$IdentityOverviewFromJson(Map<String, dynamic> json) => IdentityOverview(
      address: json['address'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      createdWithClient: json['createdWithClient'] as String,
      identityVersion: (json['identityVersion'] as num).toInt(),
      numberOfDevices: (json['numberOfDevices'] as num).toInt(),
      tier: Tier.fromJson(json['tier']),
      lastLoginAt: json['lastLoginAt'] == null ? null : DateTime.parse(json['lastLoginAt'] as String),
      datawalletVersion: (json['datawalletVersion'] as num?)?.toInt(),
    );

Map<String, dynamic> _$IdentityOverviewToJson(IdentityOverview instance) => <String, dynamic>{
      'address': instance.address,
      'createdAt': instance.createdAt.toIso8601String(),
      'createdWithClient': instance.createdWithClient,
      'identityVersion': instance.identityVersion,
      'numberOfDevices': instance.numberOfDevices,
      'tier': instance.tier,
      'lastLoginAt': instance.lastLoginAt?.toIso8601String(),
      'datawalletVersion': instance.datawalletVersion,
    };

DeletionProcess _$DeletionProcessFromJson(Map<String, dynamic> json) => DeletionProcess(
      id: json['id'] as String,
      status: json['status'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      approvalPeriodEndsAt: DateTime.parse(json['approvalPeriodEndsAt'] as String),
      approvalReminder1SentAt: json['approvalReminder1SentAt'] == null ? null : DateTime.parse(json['approvalReminder1SentAt'] as String),
      approvalReminder2SentAt: json['approvalReminder2SentAt'] == null ? null : DateTime.parse(json['approvalReminder2SentAt'] as String),
      approvalReminder3SentAt: json['approvalReminder3SentAt'] == null ? null : DateTime.parse(json['approvalReminder3SentAt'] as String),
      approvedAt: json['approvedAt'] == null ? null : DateTime.parse(json['approvedAt'] as String),
      approvedByDevice: json['approvedByDevice'] as String?,
      gracePeriodEndsAt: json['gracePeriodEndsAt'] == null ? null : DateTime.parse(json['gracePeriodEndsAt'] as String),
      gracePeriodReminder1SentAt: json['gracePeriodReminder1SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder1SentAt'] as String),
      gracePeriodReminder2SentAt: json['gracePeriodReminder2SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder2SentAt'] as String),
      gracePeriodReminder3SentAt: json['gracePeriodReminder3SentAt'] == null ? null : DateTime.parse(json['gracePeriodReminder3SentAt'] as String),
      auditLogs: (json['auditLogs'] as List<dynamic>?)?.map(AuditLog.fromJson).toList(),
    );

Map<String, dynamic> _$DeletionProcessToJson(DeletionProcess instance) => <String, dynamic>{
      'id': instance.id,
      'status': instance.status,
      'createdAt': instance.createdAt.toIso8601String(),
      'approvalPeriodEndsAt': instance.approvalPeriodEndsAt.toIso8601String(),
      'approvalReminder1SentAt': instance.approvalReminder1SentAt?.toIso8601String(),
      'approvalReminder2SentAt': instance.approvalReminder2SentAt?.toIso8601String(),
      'approvalReminder3SentAt': instance.approvalReminder3SentAt?.toIso8601String(),
      'approvedAt': instance.approvedAt?.toIso8601String(),
      'approvedByDevice': instance.approvedByDevice,
      'gracePeriodEndsAt': instance.gracePeriodEndsAt?.toIso8601String(),
      'gracePeriodReminder1SentAt': instance.gracePeriodReminder1SentAt?.toIso8601String(),
      'gracePeriodReminder2SentAt': instance.gracePeriodReminder2SentAt?.toIso8601String(),
      'gracePeriodReminder3SentAt': instance.gracePeriodReminder3SentAt?.toIso8601String(),
      'auditLogs': instance.auditLogs,
    };

AuditLog _$AuditLogFromJson(Map<String, dynamic> json) => AuditLog(
      id: json['id'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      newStatus: json['newStatus'] as String,
      messageKey: json['messageKey'] as String,
      oldStatus: json['oldStatus'] as String?,
      additionalData: json['additionalData'] as Map<String, dynamic>?,
    );

Map<String, dynamic> _$AuditLogToJson(AuditLog instance) => <String, dynamic>{
      'id': instance.id,
      'createdAt': instance.createdAt.toIso8601String(),
      'oldStatus': instance.oldStatus,
      'newStatus': instance.newStatus,
      'additionalData': instance.additionalData,
      'messageKey': instance.messageKey,
    };
