// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'change_client_secret_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ChangeClientSecretResponse _$ChangeClientSecretResponseFromJson(
  Map<String, dynamic> json,
) => ChangeClientSecretResponse(
  clientId: json['clientId'] as String,
  displayName: json['displayName'] as String,
  clientSecret: json['clientSecret'] as String,
  defaultTier: json['defaultTier'] as String,
  createdAt: DateTime.parse(json['createdAt'] as String),
  maxIdentities: (json['maxIdentities'] as num?)?.toInt(),
);

Map<String, dynamic> _$ChangeClientSecretResponseToJson(
  ChangeClientSecretResponse instance,
) => <String, dynamic>{
  'clientId': instance.clientId,
  'displayName': instance.displayName,
  'clientSecret': instance.clientSecret,
  'defaultTier': instance.defaultTier,
  'createdAt': instance.createdAt.toIso8601String(),
  'maxIdentities': instance.maxIdentities,
};
