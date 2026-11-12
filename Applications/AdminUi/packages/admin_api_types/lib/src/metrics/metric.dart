import 'package:json_annotation/json_annotation.dart';

part 'metric.g.dart';

@JsonSerializable()
class Metric {
  final String key;
  final String displayName;

  new({required this.key, required this.displayName});

  factory fromJson(dynamic json) => _$MetricFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MetricToJson(this);
}
