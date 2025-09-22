// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tier_details.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TierDetails _$TierDetailsFromJson(Map<String, dynamic> json) => TierDetails(
  id: json['id'] as String,
  name: json['name'] as String,
  quotas: (json['quotas'] as List<dynamic>).map(TierQuotaDefinition.fromJson).toList(),
);

Map<String, dynamic> _$TierDetailsToJson(TierDetails instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'quotas': instance.quotas,
};
