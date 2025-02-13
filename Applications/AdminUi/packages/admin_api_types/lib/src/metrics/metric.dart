import 'package:json_annotation/json_annotation.dart';

part 'metric.g.dart';

@JsonSerializable()
class Metric {
  final String key;
  final String displayName;

  Metric({required this.key, required this.displayName});

  factory Metric.fromJson(dynamic json) => _$MetricFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MetricToJson(this);
}
