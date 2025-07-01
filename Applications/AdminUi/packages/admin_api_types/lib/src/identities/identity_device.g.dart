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
  lastLogin: json['lastLogin'] == null ? null : LastLoginInformation.fromJson(json['lastLogin']),
);

Map<String, dynamic> _$IdentityDeviceToJson(IdentityDevice instance) => <String, dynamic>{
  'id': instance.id,
  'username': instance.username,
  'createdAt': instance.createdAt.toIso8601String(),
  'createdByDevice': instance.createdByDevice,
  'lastLogin': instance.lastLogin,
  'communicationLanguage': instance.communicationLanguage,
};

LastLoginInformation _$LastLoginInformationFromJson(Map<String, dynamic> json) => LastLoginInformation(
  time: DateTime.parse(json['time'] as String),
);

Map<String, dynamic> _$LastLoginInformationToJson(LastLoginInformation instance) => <String, dynamic>{
  'time': instance.time.toIso8601String(),
};
