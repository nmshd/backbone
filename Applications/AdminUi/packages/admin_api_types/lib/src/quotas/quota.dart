import 'package:json_annotation/json_annotation.dart';

part 'quota.g.dart';

@JsonSerializable()
class Quota {
  final String id;
  final Metric metric;
  final int max;
  final String period;

  Quota({
    required this.id,
    required this.metric,
    required this.max,
    required this.period,
  });

  factory Quota.fromJson(dynamic json) => _$QuotaFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$QuotaToJson(this);
}

@JsonSerializable()
class Metric {
  final String key;
  final String displayName;

  Metric({
    required this.key,
    required this.displayName,
  });

  factory Metric.fromJson(dynamic json) => _$MetricFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MetricToJson(this);
}
