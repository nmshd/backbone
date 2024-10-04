// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_device.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IdentityDevice _$IdentityDeviceFromJson(Map<String, dynamic> json) => IdentityDevice(
      id: json['id'] as String,
      username: json['username'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      createdByDevice: json['createdByDevice'] as String,
      communicationLanguage: json['communicationLanguage'] as String,
      lastLogin: json['lastLogin'] as Map<String, dynamic>?,
    );

Map<String, dynamic> _$IdentityDeviceToJson(IdentityDevice instance) => <String, dynamic>{
      'id': instance.id,
      'username': instance.username,
      'createdAt': instance.createdAt.toIso8601String(),
      'createdByDevice': instance.createdByDevice,
      'lastLogin': instance.lastLogin,
      'communicationLanguage': instance.communicationLanguage,
    };
