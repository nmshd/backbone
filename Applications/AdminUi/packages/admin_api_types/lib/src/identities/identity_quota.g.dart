// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'identity_quota.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IdentityQuota _$IdentityQuotaFromJson(Map<String, dynamic> json) => IdentityQuota(
  id: json['id'] as String,
  source: json['source'] as String,
  metric: Metric.fromJson(json['metric']),
  max: (json['max'] as num).toInt(),
  usage: (json['usage'] as num).toInt(),
  period: json['period'] as String,
);

Map<String, dynamic> _$IdentityQuotaToJson(IdentityQuota instance) => <String, dynamic>{
  'id': instance.id,
  'source': instance.source,
  'metric': instance.metric,
  'max': instance.max,
  'usage': instance.usage,
  'period': instance.period,
};
