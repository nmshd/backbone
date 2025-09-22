// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'individual_quota.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

IndividualQuota _$IndividualQuotaFromJson(Map<String, dynamic> json) => IndividualQuota(
  id: json['id'] as String,
  metric: Metric.fromJson(json['metric']),
  max: (json['max'] as num).toInt(),
  period: json['period'] as String,
);

Map<String, dynamic> _$IndividualQuotaToJson(IndividualQuota instance) => <String, dynamic>{
  'id': instance.id,
  'metric': instance.metric,
  'max': instance.max,
  'period': instance.period,
};
