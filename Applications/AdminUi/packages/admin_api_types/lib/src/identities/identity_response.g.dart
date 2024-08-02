// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IdentityResponse _$IdentityResponseFromJson(Map<String, dynamic> json) => IdentityResponse(
      address: json['address'] as String,
      clientId: json['clientId'] as String,
      publicKey: json['publicKey'] as String,
      tierId: json['tierId'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      identityVersion: (json['identityVersion'] as num).toInt(),
      numberOfDevices: (json['numberOfDevices'] as num).toInt(),
      devices: (json['devices'] as List<dynamic>?)?.map(IdentityDevice.fromJson).toList(),
      quotas: (json['quotas'] as List<dynamic>?)?.map(IdentityQuota.fromJson).toList(),
    );

Map<String, dynamic> _$IdentityResponseToJson(IdentityResponse instance) => <String, dynamic>{
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
