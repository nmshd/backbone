// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quota.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Quota _$QuotaFromJson(Map<String, dynamic> json) => Quota(
      id: json['id'] as String,
      metric: Metric.fromJson(json['metric']),
      max: json['max'] as int,
      period: json['period'] as String,
    );

Map<String, dynamic> _$QuotaToJson(Quota instance) => <String, dynamic>{
      'id': instance.id,
      'metric': instance.metric,
      'max': instance.max,
      'period': instance.period,
    };

Metric _$MetricFromJson(Map<String, dynamic> json) => Metric(
      key: json['key'] as String,
      displayName: json['displayName'] as String,
    );

Map<String, dynamic> _$MetricToJson(Metric instance) => <String, dynamic>{
      'key': instance.key,
      'displayName': instance.displayName,
    };
