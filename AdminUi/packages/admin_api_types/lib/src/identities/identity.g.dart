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
      devices: (json['devices'] as List<dynamic>).map(Device.fromJson).toList(),
      quotas: (json['quotas'] as List<dynamic>)
          .map(IdentityQuota.fromJson)
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
      lastLogin: LastLoginInformation.fromJson(json['lastLogin']),
    );

Map<String, dynamic> _$DeviceToJson(Device instance) => <String, dynamic>{
      'id': instance.id,
      'username': instance.username,
      'createdAt': instance.createdAt.toIso8601String(),
      'createdByDevice': instance.createdByDevice,
      'lastLogin': instance.lastLogin,
    };

LastLoginInformation _$LastLoginInformationFromJson(
        Map<String, dynamic> json) =>
    LastLoginInformation(
      time: DateTime.parse(json['time'] as String),
    );

Map<String, dynamic> _$LastLoginInformationToJson(
        LastLoginInformation instance) =>
    <String, dynamic>{
      'time': instance.time.toIso8601String(),
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

IdentityOverviewFilter _$IdentityOverviewFilterFromJson(
        Map<String, dynamic> json) =>
    IdentityOverviewFilter(
      numberOfDevices: NumberFilter.fromJson(
          json['numberOfDevices'] as Map<String, dynamic>),
      createdAt: DateFilter.fromJson(json['createdAt'] as Map<String, dynamic>),
      lastLoginAt:
          DateFilter.fromJson(json['lastLoginAt'] as Map<String, dynamic>),
      datawalletVersion: NumberFilter.fromJson(
          json['datawalletVersion'] as Map<String, dynamic>),
      identityVersion: NumberFilter.fromJson(
          json['identityVersion'] as Map<String, dynamic>),
      address: json['address'] as String?,
      tiers:
          (json['tiers'] as List<dynamic>?)?.map((e) => e as String).toList(),
      clients:
          (json['clients'] as List<dynamic>?)?.map((e) => e as String).toList(),
    );

Map<String, dynamic> _$IdentityOverviewFilterToJson(
        IdentityOverviewFilter instance) =>
    <String, dynamic>{
      'address': instance.address,
      'tiers': instance.tiers,
      'clients': instance.clients,
      'numberOfDevices': instance.numberOfDevices,
      'createdAt': instance.createdAt,
      'lastLoginAt': instance.lastLoginAt,
      'datawalletVersion': instance.datawalletVersion,
      'identityVersion': instance.identityVersion,
    };

NumberFilter _$NumberFilterFromJson(Map<String, dynamic> json) => NumberFilter(
      operator: json['operator'] as String?,
      value: json['value'] as int?,
    );

Map<String, dynamic> _$NumberFilterToJson(NumberFilter instance) =>
    <String, dynamic>{
      'operator': instance.operator,
      'value': instance.value,
    };

DateFilter _$DateFilterFromJson(Map<String, dynamic> json) => DateFilter(
      operator: json['operator'] as String?,
      value: json['value'] == null
          ? null
          : DateTime.parse(json['value'] as String),
    );

Map<String, dynamic> _$DateFilterToJson(DateFilter instance) =>
    <String, dynamic>{
      'operator': instance.operator,
      'value': instance.value?.toIso8601String(),
    };
