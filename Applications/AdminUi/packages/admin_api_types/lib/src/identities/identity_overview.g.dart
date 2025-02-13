// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

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
