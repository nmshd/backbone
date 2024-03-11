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
      identityVersion: json['identityVersion'] as int,
      numberOfDevices: json['numberOfDevices'] as int,
      devices:
          (json['devices'] as List<dynamic>?)?.map(Device.fromJson).toList(),
      quotas: (json['quotas'] as List<dynamic>?)
          ?.map(IdentityQuota.fromJson)
          .toList(),
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

IdentityQuota _$IdentityQuotaFromJson(Map<String, dynamic> json) =>
    IdentityQuota(
      id: json['id'] as String,
      source: json['source'] as String,
      metric: Metric.fromJson(json['metric']),
      max: json['max'] as int,
      usage: json['usage'] as int,
      period: json['period'] as String,
    );

Map<String, dynamic> _$IdentityQuotaToJson(IdentityQuota instance) =>
    <String, dynamic>{
      'id': instance.id,
      'source': instance.source,
      'metric': instance.metric,
      'max': instance.max,
      'usage': instance.usage,
      'period': instance.period,
    };

IdentityOverview _$IdentityOverviewFromJson(Map<String, dynamic> json) =>
    IdentityOverview(
      address: json['address'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      createdWithClient: json['createdWithClient'] as String,
      identityVersion: json['identityVersion'] as int,
      numberOfDevices: json['numberOfDevices'] as int,
      tier: Tier.fromJson(json['tier']),
      lastLoginAt: json['lastLoginAt'] == null
          ? null
          : DateTime.parse(json['lastLoginAt'] as String),
      datawalletVersion: json['datawalletVersion'] as int?,
    );

Map<String, dynamic> _$IdentityOverviewToJson(IdentityOverview instance) =>
    <String, dynamic>{
      'address': instance.address,
      'createdAt': instance.createdAt.toIso8601String(),
      'lastLoginAt': instance.lastLoginAt?.toIso8601String(),
      'createdWithClient': instance.createdWithClient,
      'datawalletVersion': instance.datawalletVersion,
      'identityVersion': instance.identityVersion,
      'numberOfDevices': instance.numberOfDevices,
      'tier': instance.tier,
    };

ODataResponse _$ODataResponseFromJson(Map<String, dynamic> json) =>
    ODataResponse(
      odataContext: json['@odata.context'] as String,
      odataCount: json['@odata.count'] as int,
      identities: (json['value'] as List<dynamic>)
          .map(IdentityOverview.fromJson)
          .toList(),
    );

Map<String, dynamic> _$ODataResponseToJson(ODataResponse instance) =>
    <String, dynamic>{
      '@odata.context': instance.odataContext,
      '@odata.count': instance.odataCount,
      'value': instance.identities,
    };
