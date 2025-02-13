// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'client.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Client _$ClientFromJson(Map<String, dynamic> json) => Client(
  clientId: json['clientId'] as String,
  displayName: json['displayName'] as String,
  defaultTier: json['defaultTier'] as String,
  createdAt: DateTime.parse(json['createdAt'] as String),
  numberOfIdentities: (json['numberOfIdentities'] as num).toInt(),
  maxIdentities: (json['maxIdentities'] as num?)?.toInt(),
);

Map<String, dynamic> _$ClientToJson(Client instance) => <String, dynamic>{
  'clientId': instance.clientId,
  'displayName': instance.displayName,
  'defaultTier': instance.defaultTier,
  'createdAt': instance.createdAt.toIso8601String(),
  'numberOfIdentities': instance.numberOfIdentities,
  'maxIdentities': instance.maxIdentities,
};
