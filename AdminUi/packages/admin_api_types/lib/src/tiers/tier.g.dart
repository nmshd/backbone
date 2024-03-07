// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tier.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Tier _$TierFromJson(Map<String, dynamic> json) => Tier(
      id: json['id'] as String,
      name: json['name'] as String,
    );

Map<String, dynamic> _$TierToJson(Tier instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
    };

TierOverview _$TierOverviewFromJson(Map<String, dynamic> json) => TierOverview(
      id: json['id'] as String,
      name: json['name'] as String,
      quotas: (json['quotas'] as List<dynamic>).map(Quota.fromJson).toList(),
    );

Map<String, dynamic> _$TierOverviewToJson(TierOverview instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'quotas': instance.quotas,
    };
