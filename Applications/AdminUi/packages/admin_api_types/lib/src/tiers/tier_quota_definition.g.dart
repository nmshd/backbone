// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tier_quota_definition.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TierQuotaDefinition _$TierQuotaDefinitionFromJson(Map<String, dynamic> json) => TierQuotaDefinition(
  id: json['id'] as String,
  metric: Metric.fromJson(json['metric']),
  max: (json['max'] as num).toInt(),
  period: json['period'] as String,
);

Map<String, dynamic> _$TierQuotaDefinitionToJson(
  TierQuotaDefinition instance,
) => <String, dynamic>{
  'id': instance.id,
  'metric': instance.metric,
  'max': instance.max,
  'period': instance.period,
};
