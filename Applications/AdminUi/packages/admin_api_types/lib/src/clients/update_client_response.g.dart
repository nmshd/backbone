// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_client_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UpdateClientResponse _$UpdateClientResponseFromJson(
  Map<String, dynamic> json,
) => UpdateClientResponse(
  clientId: json['clientId'] as String,
  displayName: json['displayName'] as String,
  defaultTier: json['defaultTier'] as String,
  createdAt: DateTime.parse(json['createdAt'] as String),
  maxIdentities: (json['maxIdentities'] as num?)?.toInt(),
);

Map<String, dynamic> _$UpdateClientResponseToJson(
  UpdateClientResponse instance,
) => <String, dynamic>{
  'clientId': instance.clientId,
  'displayName': instance.displayName,
  'defaultTier': instance.defaultTier,
  'createdAt': instance.createdAt.toIso8601String(),
  'maxIdentities': instance.maxIdentities,
};
