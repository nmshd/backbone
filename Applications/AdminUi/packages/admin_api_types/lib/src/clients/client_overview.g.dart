// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'client_overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ClientOverview _$ClientOverviewFromJson(Map<String, dynamic> json) => ClientOverview(
  clientId: json['clientId'] as String,
  displayName: json['displayName'] as String,
  defaultTier: Tier.fromJson(json['defaultTier']),
  createdAt: DateTime.parse(json['createdAt'] as String),
  maxIdentities: (json['maxIdentities'] as num?)?.toInt(),
  numberOfIdentities: (json['numberOfIdentities'] as num).toInt(),
);

Map<String, dynamic> _$ClientOverviewToJson(ClientOverview instance) => <String, dynamic>{
  'clientId': instance.clientId,
  'displayName': instance.displayName,
  'defaultTier': instance.defaultTier,
  'createdAt': instance.createdAt.toIso8601String(),
  'maxIdentities': instance.maxIdentities,
  'numberOfIdentities': instance.numberOfIdentities,
};
