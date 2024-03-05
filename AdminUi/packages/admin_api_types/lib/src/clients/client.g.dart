// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'client.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Client _$ClientFromJson(Map<String, dynamic> json) => Client(
      clientId: json['clientId'] as String,
      displayName: json['displayName'] as String,
      defaultTier: json['defaultTier'] as String,
      createdAt: json['createdAt'] as String,
      maxIdentities: json['maxIdentities'] as int?,
      numberOfIdentities: json['numberOfIdentities'] as int?,
    );

Map<String, dynamic> _$ClientToJson(Client instance) => <String, dynamic>{
      'clientId': instance.clientId,
      'displayName': instance.displayName,
      'defaultTier': instance.defaultTier,
      'createdAt': instance.createdAt,
      'maxIdentities': instance.maxIdentities,
      'numberOfIdentities': instance.numberOfIdentities,
    };

Clients _$ClientsFromJson(Map<String, dynamic> json) => Clients(
      clientId: json['clientId'] as String,
      displayName: json['displayName'] as String,
      defaultTier: ClientDefaultTier.fromJson(json['defaultTier'] as Map<String, dynamic>),
      createdAt: json['createdAt'] as String,
      maxIdentities: json['maxIdentities'] as int?,
      numberOfIdentities: json['numberOfIdentities'] as int?,
    );

Map<String, dynamic> _$ClientsToJson(Clients instance) => <String, dynamic>{
      'clientId': instance.clientId,
      'displayName': instance.displayName,
      'defaultTier': instance.defaultTier,
      'createdAt': instance.createdAt,
      'maxIdentities': instance.maxIdentities,
      'numberOfIdentities': instance.numberOfIdentities,
    };

ClientDefaultTier _$ClientDefaultTierFromJson(Map<String, dynamic> json) => ClientDefaultTier(
      id: json['id'] as String,
      name: json['name'] as String,
    );

Map<String, dynamic> _$ClientDefaultTierToJson(ClientDefaultTier instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
    };
